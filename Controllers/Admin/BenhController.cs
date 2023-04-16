using Hangfire;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement_Entities.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Controllers.Admin
{
    public class BenhController : BaseController
    {
        private readonly IBenh _service;
        private readonly ITienIch _tienichRep;
        public BenhController(IBenh service, ITienIch tienichRep)
        {
            _service = service;
            _tienichRep = tienichRep;
        }

        public async Task<IActionResult> Index(BenhSearchModel model)
        {

            if (!model.Page.HasValue) model.Page = 1;
            var listPaged = await _service.SearchByCondition(model);
            ViewBag.MaCK = await _service.ChuyenKhoaNav();


            ViewBag.Names = listPaged;
            ViewBag.Data = model;
            return View(new BenhSearchModel());
        }


        [HttpGet]

        public async Task<IActionResult> PageList(BenhSearchModel model)
        {

            var listmodel = await _service.SearchByCondition(model);
            if (listmodel.Count() > 0)
            {

                if (!model.Page.HasValue) model.Page = 1;

                ViewBag.Names = listmodel;
                ViewBag.Data = model;

                return PartialView("_NameListPartial", listmodel);
            }
            else
            {

                return Json(new { status = -2, title = "", text = "Không tìm thấy", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            }


        }
        public IActionResult addCTTrieuChung()
        {

            return PartialView("_CTTrieuChungView", new CTrieuChungModel());
        }


        public async Task<IActionResult> Add()
        {
            ViewBag.MaCK = new SelectList(await _service.ChuyenKhoaNav(), "MaCK", "TenCK");

            return PartialView("_partialAdd", new Benh());

        }
        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Benh model, CTrieuChungModel[] Trieuchungs)
        {
            if (ModelState.IsValid)
            {
                if (Trieuchungs.Count() >= 1)
                {
                    model.MaBenh = Guid.NewGuid();
                    var result = await _service.Add(model, Trieuchungs.ToList());
                    if (result.errorCode == -1)
                    {
                        ViewBag.MaCK = new SelectList(await _service.ChuyenKhoaNav(), "MaCK", "TenCK");
                        ModelState.AddModelError("TenBenh", "Tên bệnh đã tồn tại");
                        return PartialView("_partialAdd", model);
                    }
                    if (result.errorCode == 0)
                        return Json(new { status = 1, title = "", text = "Thêm thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
                    else
                        return Json(new { status = -2, title = "", text = "Thêm không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
                }
                else
                    return Json(new { status = -3, title = "", text = "Vui lòng thêm ít nhất một triệu chứng", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());

            }
            ViewBag.MaCK = new SelectList(await _service.ChuyenKhoaNav(), "MaCK", "TenCK");
            return PartialView("_partialAdd", model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _service.Get(id);
            if (item == null)
            {
                return NotFound(); ;
            }
            else
            {
                var listTC = new List<CTrieuChungModel>();
                ViewBag.MaCK = new SelectList(await _service.ChuyenKhoaNav(), "MaCK", "TenCK", (await _service.Get(id)).MaCK);
                
                foreach (var trieuchung in item.CTTrieuChung)
                {
                    listTC.Add(new CTrieuChungModel { MaBenh = item.MaBenh, MaTrieuChung = trieuchung.MaTrieuChung, TenTrieuChung = trieuchung.MaTrieuChungNavigation.TenTrieuChung });
                }
                ViewBag.ListTC = listTC;
                return PartialView("_partialedit", await _service.Get(id));
            }

        }


        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            if (await _service.Get(id) == null)
            {
                return NotFound(); ;
            }
            else
            {
                ViewBag.MaCK = new SelectList(await _service.ChuyenKhoaNav(), "MaCK", "TenCK", (await _service.Get(id)).MaCK);


                return PartialView("_partialDetail", await _service.Get(id));
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Benh model, CTrieuChungModel[] Trieuchungs)
        {
            if (ModelState.IsValid)
            {
                if (Trieuchungs.Count() >= 1)
                {
                    var result = await _service.Edit(model, Trieuchungs.ToList());
                    if (result.errorCode == -1)
                    {
                        ViewBag.MaCK = new SelectList(await _service.ChuyenKhoaNav(), "MaCK", "TenCK");
                        ViewBag.ListTC = Trieuchungs.ToList();
                        ModelState.AddModelError("TenBenh", "Tên bệnh đã tồn tại");
                        return PartialView("_partialedit", model);
                    }
                    if (result.errorCode == 0)
                    {
                        using (new BackgroundJobServer())
                        {
                            _tienichRep.refreshCacheBenh();
                            _tienichRep.refreshCacheTrieuChung();
                            return Json(new { status = 1, title = "", text = "Cập nhật thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
                        }
                    }

                    else
                        return Json(new { status = -2, title = "", text = "Cập nhật không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
                }
                else
                    return Json(new { status = -3, title = "", text = "Vui lòng thêm ít nhất một triệu chứng", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            }
            ViewBag.MaCK = new SelectList(await _service.ChuyenKhoaNav(), "MaCK", "TenCK");
            ViewBag.ListTC = Trieuchungs.ToList();
            return PartialView("_partialedit", model);

        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _service.Delete(id))
                return Json(new { status = 1, title = "", text = "Xoá thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            else
                return Json(new { status = -2, title = "", text = "Xoá không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
        }
    }
}
