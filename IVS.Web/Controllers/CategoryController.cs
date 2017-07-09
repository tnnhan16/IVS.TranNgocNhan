using IVS.BL;
using IVS.Components;
using IVS.Models.Model;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace IVS.Web.Controllers
{
    public class CategoryController : Controller
    {
        public CategoryBL _categoryBL;
        public CategoryController()
        {
            _categoryBL = new CategoryBL();
        }

        public ActionResult Index(int? page)
        {
            ModelSearch ModelSearch = new ModelSearch();
            ModelSearch.code= Request.Form["code"];
            ModelSearch.name = Request.Form["name"];
            int Out;
           bool checkparent= int.TryParse(Request.Form["parent"],out Out);
            if(checkparent == true)
            {
                ModelSearch.parent_id = int.Parse(Request.Form["parent"]);
            }
            bool checkchild = int.TryParse(Request.Form["child"], out Out);
            if (checkchild == true)
            {
                ModelSearch.Child_id = int.Parse(Request.Form["child"]);
            }
            ModelSearch.parent_id = 0;
             var pageNumber = page ?? 1;
            List<CategoryViewModel> model = new List<CategoryViewModel>();
             if(ModelSearch.name == null && ModelSearch.code == null && ModelSearch.parent_id ==0)
            {
                model = _categoryBL.GetAll();
            }
             else
            {
                int result = _categoryBL.Search(ModelSearch, out model);
            }
           var  List = model.ToPagedList(pageNumber, 13);
            ViewBag.ListSearch = model.OrderByDescending(x=>x.id);
            return View(new Tuple<ModelSearch, IPagedList<CategoryViewModel>>(ModelSearch, List));
        }

        //POST: Category/Index/5
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Index(ModelSearch model,int? page)
        {
            var pagenumber = page ?? 1;
            List<CategoryViewModel> List = new List<CategoryViewModel>();

            int result = _categoryBL.Search(model, out List);

            if (List!=null)
            {
               var ListPage = List.ToPagedList(pagenumber, 20);
                ViewBag.ListSearch = List.OrderByDescending(x => x.id);
                return View(new Tuple<ModelSearch, IPagedList<CategoryViewModel>>(model, ListPage));
            }
            return View();

        }
        #region ADD
        public ActionResult Add()
        {
            CategoryModel Model = new CategoryModel();
            ViewBag.Parent = new SelectList(_categoryBL.GetListParent(), "id", "name");
            return View(Model);
        }

        [HttpPost]
        public ActionResult Add(CategoryModel Model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Parent = new SelectList(_categoryBL.GetListParent(), "id", "name");
                return View(Model);
            }
            List<string> lstMsg = new List<string>();
            int returnCode = _categoryBL.Insert(Model, out lstMsg);
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                ViewBag.Parent = new SelectList(_categoryBL.GetListParent(), "id", "name");
                if (lstMsg != null)
                {
                    for (int i = 0; i < lstMsg.Count(); i++)
                    {
                        ModelState.AddModelError(string.Empty, lstMsg[i]);
                    }
                }
                return View(Model);
            }
            TempData["Success"] = "Inserted Successfully!";
            return RedirectToAction("Index");
        }
        #endregion
        #region View & Edit
        public ActionResult View(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }

            CategoryViewModel Model = new CategoryViewModel();
            int returnCode = _categoryBL.GetDetail(long.Parse(id), out Model);
            if (Model == null)
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                Model = new CategoryViewModel();
            }

            return View(Model);
        }
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }

            CategoryModel Model = new CategoryModel();
            int returnCode = _categoryBL.GetByID(long.Parse(id), out Model);
            if (Model == null)
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                Model = new CategoryModel();
            }
            ViewBag.Parent = new SelectList(_categoryBL.GetListParent(), "id", "name");
            return View(Model);
        }
        [HttpPost]
        public ActionResult Edit(CategoryModel Model)
        {
            if (ModelState.IsValid)
            {
                List<string> lstMsg = new List<string>();
                int returnCode = _categoryBL.Update(Model, out lstMsg);

                if (!((int)Common.ReturnCode.Succeed == returnCode))
                {
                    if (lstMsg != null)
                    {
                        for (int i = 0; i < lstMsg.Count(); i++)
                        {
                            ModelState.AddModelError(string.Empty, lstMsg[i]);
                        }
                    }
                    ViewBag.Parent = new SelectList(_categoryBL.GetListParent(), "id", "name");
                    return View(Model);
                }
                TempData["Success"] = "Updated Successfully!";
                return RedirectToAction("View", new { @id = Model.id });
            }
            ViewBag.Parent = new SelectList(_categoryBL.GetListParent(), "id", "name");
            return View(Model);
        }
        #endregion

        //POST: Category/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            List<string> lstMsg = new List<string>();

            int returnCode = _categoryBL.Delete(id, out lstMsg);
            if (((int)Common.ReturnCode.Succeed == returnCode))
            {
                return Json(new { Message = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = false }, JsonRequestBehavior.AllowGet);
            }
        }
    
        //Get list parent
        public JsonResult GetParent()
        {

            var model = _categoryBL.GetParent();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategory(long id)
        {
            var model = _categoryBL.GetCategory(id);
            ComboboxParent item = new ComboboxParent();
            item.id = 0;
            item.name = "--Select child-- ";                
            model.Add(item);              
            return Json(model.OrderBy(x=>x.id), JsonRequestBehavior.AllowGet);
        }
    }
}
