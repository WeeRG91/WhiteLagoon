using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{

    public class VillaNumberController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public VillaNumberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var villaNumbers = _unitOfWork.VillaNumber.GetAll(includeProperties: "Villa");
            return View(villaNumbers);
        }

        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(villa => new SelectListItem
                {
                    Text = villa.Name,
                    Value = villa.Id.ToString()
                }),
            };

            //IEnumerable<SelectListItem> list = _db.Villas.ToList().Select(villa => new SelectListItem
            //{
            //    Text = villa.Name,
            //    Value = villa.Id.ToString()
            //});
            //ViewData["list"] = list; //type of distionary
            //ViewBag.list = list;

            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Create(VillaNumberVM obj)
        {
            //ModelState.Remove("Villa");
            if (obj.VillaNumber == null)
            {
                TempData["error"] = "Villa number details are missing.";
                obj.VillaList = _unitOfWork.Villa.GetAll().Select(villa => new SelectListItem
                {
                    Text = villa.Name,
                    Value = villa.Id.ToString()
                });
                return View(obj);
            }

            bool roomNumberExists = _unitOfWork.VillaNumber.Any(villaNumber => villaNumber.Villa_Number == obj.VillaNumber.Villa_Number);

            if (ModelState.IsValid && !roomNumberExists)
            {
                _unitOfWork.VillaNumber.Add(obj.VillaNumber);
                _unitOfWork.Save();

                TempData["success"] = "The villa number " + obj.VillaNumber.Villa_Number + " has been created successfully.";

                return RedirectToAction("Index", "VillaNumber");
            }

            if (roomNumberExists)
            {
                TempData["error"] = "The villa number " + obj.VillaNumber.Villa_Number + " already exists.";
            }

            obj.VillaList = _unitOfWork.Villa.GetAll().Select(villa => new SelectListItem
            {
                Text = villa.Name,
                Value = villa.Id.ToString()
            });

            return View(obj);
        }

        public IActionResult Update(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(villa => new SelectListItem
                {
                    Text = villa.Name,
                    Value = villa.Id.ToString()
                }),
                VillaNumber = _unitOfWork.VillaNumber.Get(villaNumber => villaNumber.Villa_Number == villaNumberId),
            };

            if (villaNumberVM.VillaNumber == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Fetch the Villa associated with the VillaNumber
            Villa? villa = _unitOfWork.Villa.Get(villa => villa.Id == villaNumberVM.VillaNumber.VillaId);

            // Set the villa name in ViewData
            if (villa != null)
            {
                ViewData["villaName"] = villa.Name;
            }

            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Update(VillaNumberVM obj)
        {
            if (obj.VillaNumber == null)
            {
                TempData["error"] = "Villa number details are missing.";
                obj.VillaList = _unitOfWork.Villa.GetAll().Select(villa => new SelectListItem
                {
                    Text = villa.Name,
                    Value = villa.Id.ToString()
                });
                return View(obj);
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.VillaNumber.Update(obj.VillaNumber);
                _unitOfWork.Save();

                TempData["success"] = "The villa number " + obj.VillaNumber.Villa_Number + " has been updated successfully.";

                return RedirectToAction("Index", "VillaNumber");
            }

            obj.VillaList = _unitOfWork.Villa.GetAll().Select(villa => new SelectListItem
            {
                Text = villa.Name,
                Value = villa.Id.ToString()
            });
            TempData["error"] = "The villa number " + obj.VillaNumber.Villa_Number + " could not be updated.";
            return View(obj);
        }

        public IActionResult Delete(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(villa => new SelectListItem
                {
                    Text = villa.Name,
                    Value = villa.Id.ToString()
                }),
                VillaNumber = _unitOfWork.VillaNumber.Get(villaNumber => villaNumber.Villa_Number == villaNumberId),
            };

            if (villaNumberVM.VillaNumber == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Fetch the Villa associated with the VillaNumber
            Villa? villa = _unitOfWork.Villa.Get(villa => villa.Id == villaNumberVM.VillaNumber.VillaId);

            // Set the villa name in ViewData
            if (villa != null)
            {
                ViewData["villaName"] = villa.Name;
            }

            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Delete(VillaNumberVM obj)
        {
            if (obj.VillaNumber == null)
            {
                TempData["error"] = "Villa number details are missing.";
                return View(nameof(Delete), "VillaNumber");
            }

            VillaNumber? objFromDb = _unitOfWork.VillaNumber.Get(villaNumber => villaNumber.Villa_Number == obj.VillaNumber.Villa_Number);

            if (objFromDb is not null)
            {
                _unitOfWork.VillaNumber.Delete(objFromDb);
                _unitOfWork.Save();

                TempData["success"] = "The villa " + obj.VillaNumber.Villa_Number + " has been deleted successfully.";

                return RedirectToAction(nameof(Index), "VillaNumber");
            }

            TempData["error"] = "The villa " + obj.VillaNumber.Villa_Number + " could not be deleted successfully.";
            return View();
        }
    }
}
