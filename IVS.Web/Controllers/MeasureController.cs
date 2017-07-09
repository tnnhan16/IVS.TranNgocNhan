﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using BL;
using IVS.Models.Model;
using IVS.Components;

namespace IVS.Web.Controllers
{
    public class MeasureController : Controller
    {
        public MeasureBLL _measureBLL;
        public MeasureController()
        {
            _measureBLL = new MeasureBLL();
        }
        // GET: Measure
        [HttpGet]
        public ActionResult Index(MeasureModel Model, int? page)
        {
            var pageNumber = page ?? 1;
            List<MeasureModel> lstModel = new List<MeasureModel>();
            int total = new int();
            if (!string.IsNullOrEmpty(Session["m_code"] as string))
            {
                Model.code = Session["m_code"].ToString();
            }
            if (!string.IsNullOrEmpty(Session["m_name"] as string))
            {
                Model.name = Session["m_name"].ToString();
            }
            _measureBLL.Search(Model, out lstModel, out total, pageNumber);
            var list = new StaticPagedList<MeasureModel>(lstModel, pageNumber, 15, total);
            ViewBag.ListSearch = lstModel.OrderByDescending(x => x.id);
            ViewBag.page = 0;
            if (page != null)
            {
                ViewBag.page = pageNumber - 1;
            }
            return View(new Tuple<MeasureModel, IPagedList<MeasureModel>>(Model, list));
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult IndexPost(MeasureModel Model, int? page)
        {
            var pageNumber = page ?? 1;
            List<MeasureModel> lstModel = new List<MeasureModel>();
            int total = new int();
            _measureBLL.Search(Model, out lstModel, out total, pageNumber);
            var list = new StaticPagedList<MeasureModel>(lstModel, pageNumber, 15, total);
            ViewBag.ListSearch = lstModel.OrderByDescending(x => x.id);
            Session["m_code"] = Model.code;
            Session["m_name"] = Model.name;
            TempData["CountResult"] = total.ToString() + " row(s) found!";
            return View(new Tuple<MeasureModel, IPagedList<MeasureModel>>(Model, list));
        }

        [HttpGet]
        public ActionResult Add()
        {
            MeasureModel Model = new MeasureModel();

            return View(Model);
        }

        [HttpPost]
        public ActionResult Add(MeasureModel Model)
        {
            List<string> lstMsg = new List<string>();
            int returnCode = _measureBLL.Insert(Model, out lstMsg);
            if (ModelState.IsValid)
            {
                if (!((int)Common.ReturnCode.Succeed == returnCode))
                {
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
            return View();
        }

        [HttpGet]
        public ActionResult View(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            MeasureModel Model = new MeasureModel();
            int returnCode = _measureBLL.GetDetail(id, out Model);
            if (Model == null)
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                Model = new MeasureModel();
            }

            return View(Model);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }

            MeasureModel Model = new MeasureModel();
            int returnCode = _measureBLL.GetDetail(id, out Model);
            if (Model == null)
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            if (!((int)Common.ReturnCode.Succeed == returnCode))
            {
                Model = new MeasureModel();
            }

            return View(Model);
        }
        [HttpPost]
        public ActionResult Edit(MeasureModel Model)
        {
            if (string.IsNullOrEmpty(Model.id.ToString()))
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            List<string> lstMsg = new List<string>();
            int returnCode = _measureBLL.Update(Model, out lstMsg);
            if (ModelState.IsValid)
            {
                if (!((int)Common.ReturnCode.Succeed == returnCode))
                {
                    if (lstMsg != null)
                    {
                        for (int i = 0; i < lstMsg.Count(); i++)
                        {
                            ModelState.AddModelError(string.Empty, lstMsg[i]);
                        }
                    }
                    return View(Model);
                }
                TempData["Success"] = "Updated Successfully!";
                return RedirectToAction("View", new { @id = Model.id });
            }
            return View();
        }
        [HttpPost]
        public ActionResult Delete(List<int> id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                TempData["Error"] = "Data has already been deleted by other user!";
                return RedirectToAction("Index");
            }
            List<string> lstMsg = new List<string>();

            int returnCode = _measureBLL.Delete(id, out lstMsg);
            if (((int)Common.ReturnCode.Succeed == returnCode))
            {
                return Json(new { Message = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}