using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AmenityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var amenities = _unitOfWork.Amenity.GetAll(includeProperties: "Villa");
            return View(amenities);
        }

        public IActionResult Create()
        {
            AmenityVM amenityVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(villa => new SelectListItem
                {
                    Text = villa.Name,
                    Value = villa.Id.ToString(),
                })
            };

            return View(amenityVM);
        }
        [HttpPost]
        public IActionResult Create(AmenityVM obj)
        {
            if (obj.Amenity == null)
            {
                TempData["error"] = "Amenity details are missing.";
                return RedirectToAction("Create", "Amenity");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Amenity.Add(obj.Amenity);
                _unitOfWork.Save();

                TempData["success"] = "The amenity " + obj.Amenity.Name + " has been created successfully.";

                return RedirectToAction("Index", "Amenity");
            }

            obj.VillaList = _unitOfWork.Villa.GetAll().Select(villa => new SelectListItem
            {
                Text = villa.Name,
                Value = villa.Id.ToString()
            });

            return View(obj);
        }

        public IActionResult Update(int amenityId)
        {
            AmenityVM amenityVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(villa => new SelectListItem
                {
                    Text = villa.Name,
                    Value = villa.Id.ToString()
                }),
                Amenity = _unitOfWork.Amenity.Get(amenity => amenity.Id == amenityId),
            };

            if (amenityVM.Amenity == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Fetch the Villa associated with the VillaNumber
            Villa? villa = _unitOfWork.Villa.Get(villa => villa.Id == amenityVM.Amenity.Id);

            // Set the villa name in ViewData
            if (villa != null)
            {
                ViewData["villaName"] = villa.Name;
            }

            return View(amenityVM);
        }

        [HttpPost]
        public IActionResult Update(AmenityVM obj)
        {
            if (obj.Amenity == null)
            {
                TempData["error"] = "Amenity details are missing.";
                obj.VillaList = _unitOfWork.Villa.GetAll().Select(villa => new SelectListItem
                {
                    Text = villa.Name,
                    Value = villa.Id.ToString()
                });
                return View(obj);
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Amenity.Update(obj.Amenity);
                _unitOfWork.Save();

                TempData["success"] = "The amenity " + obj.Amenity.Name + " has been updated successfully.";

                return RedirectToAction("Index", "Amenity");
            }

            obj.VillaList = _unitOfWork.Villa.GetAll().Select(villa => new SelectListItem
            {
                Text = villa.Name,
                Value = villa.Id.ToString()
            });

            TempData["error"] = "The amenity " + obj.Amenity.Name + " could not be updated.";
            return View(obj);
        }

        public IActionResult Delete(int amenityId)
        {
            AmenityVM amenityVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(villa => new SelectListItem
                {
                    Text = villa.Name,
                    Value = villa.Id.ToString()
                }),
                Amenity = _unitOfWork.Amenity.Get(amenity => amenity.Id == amenityId),
            };

            if (amenityVM.Amenity == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Fetch the Villa associated with the VillaNumber
            Villa? villa = _unitOfWork.Villa.Get(villa => villa.Id == amenityVM.Amenity.Id);

            // Set the villa name in ViewData
            if (villa != null)
            {
                ViewData["villaName"] = villa.Name;
            }

            return View(amenityVM);
        }

        [HttpPost]
        public IActionResult Delete(AmenityVM obj)
        {
            if (obj.Amenity == null)
            {
                TempData["error"] = "Amenity details are missing.";
                return View(nameof(Delete), "Amenity");
            }

            Amenity? objFromDb = _unitOfWork.Amenity.Get(amenity => amenity.Id == obj.Amenity.Id);

            if (objFromDb is not null)
            {
                _unitOfWork.Amenity.Delete(objFromDb);
                _unitOfWork.Save();

                TempData["success"] = "The amenity " + obj.Amenity.Name + " has been deleted successfully.";

                return RedirectToAction(nameof(Index), "Amenity");
            }

            TempData["error"] = "The amenity " + obj.Amenity.Name + " could not be deleted successfully.";
            return View();
        }
    }
}
