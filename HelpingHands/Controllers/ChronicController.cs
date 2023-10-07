using HelpingHands.Data;
using HelpingHands.Models;
using HelpingHands.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HelpingHands.Controllers
{
    public class ChronicController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ChronicController(ApplicationDbContext context) 
        {
            _context = context;      
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ChronicCondition()
        {
            var chronicCond= _context.ChronicCondition.Where(cc=>cc.Archived==false).ToList();

            return View(chronicCond);
        }
        public IActionResult AddCondition()
        {
            return View();

        }

        public async Task<IActionResult> SubmitCondition(AddChronicVM model) 
        {
            if (ModelState.IsValid)
            {
                bool checkChronic = CheckChronicExist(model.Name);
                if (checkChronic==false)
                {

                    var newCondition = new ChronicCondition
                    {
                        Name = model.Name,
                        Description = model.Description,
                    };

                    _context.ChronicCondition.Add(newCondition);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ChronicCondition", "Chronic");
                }
                else
                {

                }
            }
        
            return View(model);
        }
        public IActionResult EditCondition([FromRoute] int id)
        {
            var chronicCond = _context.ChronicCondition.Where(cc=>cc.ChronicID==id).FirstOrDefault();

            var editCondition = new EditChronicVM
            {
                ChronicID = chronicCond.ChronicID,
                Name = chronicCond.Name,
                Description = chronicCond.Description,
            };

            return View(editCondition);
        }
        public async Task<IActionResult> UpdateCondition(EditChronicVM model)
        {
            if (ModelState.IsValid)
            {
                var chronicCond = _context.ChronicCondition.Where(cc => cc.ChronicID == model.ChronicID).FirstOrDefault();
                if (chronicCond == null)
                {
                    return NotFound();
                }

                chronicCond.Name = model.Name;
                chronicCond.Description= model.Description; 

                _context.ChronicCondition.Update(chronicCond);
                await _context.SaveChangesAsync();

                return RedirectToAction("ChronicCondition", "Chronic");
            }
            return View(model);
        }
        [HttpPost("/Chronic/DeleteCondition/{chronicID}")]
        public JsonResult DeleteSuburb(int chronicID)
        {
            var chronicCond = _context.ChronicCondition.Where(cc => cc.ChronicID == chronicID).FirstOrDefault();

            chronicCond.Archived = true;

            _context.ChronicCondition.Update(chronicCond);
            _context.SaveChanges();

            return Json(true);
        }
        public bool CheckChronicExist(string condition)
        {
            return _context.ChronicCondition.Any(c => c.Name == condition);
        }


    }
}
