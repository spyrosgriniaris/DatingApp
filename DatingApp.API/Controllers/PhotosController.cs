using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repo = repo;
            // gia na ferw to cloudinary configuration kanw xrisi tou options

            Account acc = new Account (
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id){
            // vazw tin getPhoto sto IDating repo kai sto Dating repo
            var photoFromRepo = await _repo.GetPhoto(id);
            
            // ftiaxnw to kainourio Dto 
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            //elegxw an tairiazei to id tou logged in user me auto tou route
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId);

            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                // to using xrisimopoieitai gia na ginei dispose oti uparxei sti mnimi molis teleiwsei i ektelesi
                using (var stream = file.OpenReadStream()){
                    var uploadParams = new ImageUploadParams(){
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            photoForCreationDto.Url = uploadResult.Url.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            // elegxw an exei alles photo. an den exei tin kanw main
            if (!userFromRepo.Photos.Any(u => u.isMain))
                photo.isMain = true;
            
            userFromRepo.Photos.Add(photo);

            

            if (await _repo.SaveAll()){
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                // ftiaxnw tin getPhoto panw
                return CreatedAtRoute("GetPhoto", new {userId = userId, id = photo.Id}, photoToReturn);
            }

            return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id){
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            // elegxw an prospathei na epeksergastei fwtografia pou anikei ston idio
            var user = await _repo.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();
            
            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.isMain)
                return BadRequest("This is already the main photo");

            // ftiaxnw ti methodo sto DatingRepo
            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            currentMainPhoto.isMain = false;

            photoFromRepo.isMain = true;

            if (await _repo.SaveAll())
                return NoContent();
            
            return BadRequest("Could not set photo to main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id){
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            // elegxw an prospathei na epeksergastei fwtografia pou anikei ston idio
            var user = await _repo.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();
            
            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.isMain)
                return BadRequest("You cannot delete your main photo");
            if (photoFromRepo.PublicId != null) {
                // tha prepei na svisw ti photo apo to cloudinary kai to reference tis apo tin vasi mou
                // create deletion params needed
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                // response of cloudinary is a string
                var result = _cloudinary.Destroy(deleteParams);
                if (result.Result == "ok"){
                    _repo.Delete(photoFromRepo);
                }
            }
            if (photoFromRepo.PublicId == null){
                 _repo.Delete(photoFromRepo);
            }
            
            if (await _repo.SaveAll())
                return Ok();
                
            return BadRequest("Failed to delete the Photo");
        }

    }
}