using Dapper;
using IVS.Models.Model;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using IVS.Components;
using System;
using PagedList;

namespace IVS.DAL.DAO
{
    public class CategoryDAO
    {
        public IDbConnection _db;
        public CategoryDAO()
        {
            _db = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString);
        }
        //Get all data
        public List<CategoryViewModel> GetAll()
        {
         
            string strQuery = "SELECT cate.`id`, cate_parent.`name` as parent_name, cate.`code`, cate.`name`, cate.`description`";
            strQuery += " FROM product_category cate LEFT JOIN(SELECT `id`, `name` FROM `product_category`) cate_parent";
            strQuery += " ON cate.`parent_id` = cate_parent.id";
            return _db.Query<CategoryViewModel>(strQuery).ToList();
          
        }
        //Get list parent for combobox
        public List<CategoryParent> GetListParent()
        {
            string strQuery = "SELECT `id`, `parent_id`, `name` FROM `product_category` ORDER BY `parent_id`";
            return _db.Query<CategoryParent>(strQuery).ToList();
        }
        //Insert data 
        public int Insert(CategoryModel model, out List<string> lstMsg)
        {
            int result = (int)Common.ReturnCode.Succeed;
            lstMsg = new List<string>();
            try
            {
                if (isError(model, (int)Common.ActionType.Add, out lstMsg))
                {
                    return (int)Common.ReturnCode.UnSuccess;
                }
                var strQuery = "INSERT INTO `product_category` (`parent_id`, `code`, `name`, `description`) VALUES(@parent_id, @code, @name, @description); ";
                _db.Execute(strQuery, model);
            }
            catch (Exception ex)
            {
                lstMsg.Add("Exception Occurred.");
                result = (int)Common.ReturnCode.UnSuccess;
            }
            return result;
        }

        public int Update(CategoryModel model, out List<string> lstMsg)
        {
            int result = (int)Common.ReturnCode.Succeed;
            lstMsg = new List<string>();

            try
            {
                if (isError(model, (int)Common.ActionType.Update, out lstMsg))
                {
                    return (int)Common.ReturnCode.UnSuccess;
                }
                string strQuery = "UPDATE `product_category` SET `parent_id` = @parent_id, `code` = @code, `name` = @name, `description` = @description, `updated_datetime` = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFFFFF") + "' WHERE `id` = @id ";
                _db.Execute(strQuery, model);
            }
            catch (Exception ex)
            {
                lstMsg.Add("Exception Occurred.");
                result = (int)Common.ReturnCode.UnSuccess;
            }

            return result;
        }
        public int GetByID(long ID, out CategoryModel Model)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            Model = new CategoryModel();
            try
            {
                if (ID != 0)
                {
                    string strQuery = "SELECT *";
                    strQuery += " FROM product_category";
                    strQuery += " WHERE `id` = @id";
                    Model = _db.Query<CategoryModel>(strQuery, new { id = ID }).SingleOrDefault();
                }
            }
            catch (Exception)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
            }

            return returnCode;
        }

        public int GetChildrens_Category(long ID, out List<CategoryModel> Childrens_Category)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
           Childrens_Category = new List<CategoryModel>();
            try
            {
                if (ID != 0)
                {
                    string strQuery = "SELECT * FROM `product_category` WHERE `parent_id` = @id";
                    var model = _db.Query<CategoryModel>(strQuery,new { id = ID }).ToList();
                    Childrens_Category = model;
                    return returnCode;
                }
            }
            catch (Exception ex)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;

            }

            return returnCode;
        }


        public int GetListCategory(long ID, out List<Product_Item_List_Delete> ListCategory)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            ListCategory = new List<Product_Item_List_Delete>();
            try
            {
                if (ID != 0)
                {
                    string strQuery = "SELECT `id`,`category_id`,`code`";
                    strQuery += " FROM product_item";
                    strQuery += " WHERE `category_id` = @id";
                    ListCategory = _db.Query<Product_Item_List_Delete>(strQuery, new { id = ID }).ToList();
                }
            }
            catch (Exception)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
            }

            return returnCode;
        }

        public int GetDetail(long ID, out CategoryViewModel Model)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            Model = new CategoryViewModel();
            try
            {
                if (ID != 0)
                {
                    string strQuery = "SELECT cate.`id`, cate_parent.`name` parent_name, cate.`code`, cate.`name`, cate.`description`";
                    strQuery += " FROM product_category cate LEFT JOIN(SELECT `id`, `name` FROM `product_category`) cate_parent";
                    strQuery += " ON cate.`parent_id` = cate_parent.id WHERE cate.`id` = @id";
                    Model = _db.Query<CategoryViewModel>(strQuery, new { id = ID }).SingleOrDefault();
                }
            }
            catch (Exception)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
            }

            return returnCode;
        }

        public int Delete(long ID, out List<string> lstMsg)
        {
            List<Product_Item_List_Delete> ListCategory = new List<Product_Item_List_Delete>();
            List<CategoryModel> Childrens_Category = new List<CategoryModel>();

            int returnCode = (int)Common.ReturnCode.Succeed;
            lstMsg = new List<string>();
            try
            {
                if (Utilities.isNullOrEmpty(ID))
                {
                    lstMsg.Add("Data has already been deleted by other user!");
                    return (int)Common.ReturnCode.UnSuccess;
                }
                if (!isError_Delete(ID.ToString(), (int)Common.ActionType.Delete, out lstMsg))
                {
                    int product_category = GetChildrens_Category(ID, out Childrens_Category);
                    _db.Open();
                    using (var _transaction = _db.BeginTransaction())
                    {
                        if (product_category == (int)Common.ReturnCode.Succeed && Childrens_Category.Count() > (int)Common.ReturnCode.Succeed)
                        {

                            foreach (var Child in Childrens_Category)
                            {
                                string strQuery = "UPDATE `product_category` SET `parent_id`=0  WHERE `id` = @id";
                                _db.Execute(strQuery, new { id = Child.id });

                                int result = GetListCategory(Child.id, out ListCategory);
                                if (result == (int)Common.ReturnCode.Succeed)
                                {
                                    foreach (var product_item in ListCategory)
                                    {
                                        string strQuer = "UPDATE `product_item` SET `category_id`=0  WHERE `code` = @code AND `id` = @id";
                                        _db.Execute(strQuer, new { code = product_item.code, id = product_item.id });
                                    }
                                }
                            }
                        }
                        else
                        {
                            int result = GetListCategory(ID, out ListCategory);
                            if (result == (int)Common.ReturnCode.Succeed)
                            {
                                foreach (var product_item in ListCategory)
                                {
                                    string strQuer = "UPDATE `product_item` SET `category_id`=0  WHERE `code` = @code AND `id` = @id";
                                   _db.Execute(strQuer, new { code = product_item.code, id = product_item.id });
                                }
                            }

                        }
                        _db.Execute("DELETE FROM `product_category` WHERE `id`=@id", new { id = ID });
                        _transaction.Commit();
                    }

                }


            }
            catch (Exception ex)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
                lstMsg = new List<string>();
            }

            return returnCode;
        }

        private bool isError(CategoryModel Model, int ActionType, out List<string> lstMessage)
        {
            bool isError = false;
            lstMessage = new List<string>();
            if (Model.code.Contains(" "))
            {
                isError = true;
                lstMessage.Add("[Code] must not contains space character!");
            }
            if ((int)Common.ActionType.Add == ActionType)
            {
                string strQuery = "SELECT `id` FROM `product_category` WHERE `code` = @code LIMIT 1 ";
                var hasItem = _db.Query<CategoryParent>(strQuery, new { code = Model.code }).ToList();
                if (hasItem.Count != 0)
                {
                    isError = true;
                    lstMessage.Add("[Code] is duplicate!");
                }
            }
            if ((int)Common.ActionType.Update == ActionType)
            {
                string strQuery = "SELECT `id` FROM `product_category` WHERE `code` = @code AND `id` <> @id LIMIT 1";
                var hasItem = _db.Query<CategoryParent>(strQuery, new { code = Model.code, id = Model.id }).ToList();
                if (hasItem.Count != 0)
                {
                    isError = true;
                    lstMessage.Add("[Code] is duplicate!");
                }
            }
            return isError;
        }

        private bool isError_Delete(string ID, int ActionType, out List<string> lstMessage)
        {
            bool isError = false;
            lstMessage = new List<string>();
            if (string.IsNullOrEmpty(ID))
            {
                isError = true;
                lstMessage.Add("[Code] must not contains space character!");
            }

            if ((int)Common.ActionType.Delete == ActionType)
            {
                string strQuery = "SELECT `id` FROM `product_category` WHERE  `id` = @id";
                var hasItem = _db.Query<CategoryParent>(strQuery, new { id = int.Parse(ID) }).ToList();
                if (hasItem.Count == 0)
                {
                    isError = true;
                    lstMessage.Add("[Category] is not found!");
                }
            }
            return isError;
        }

        public int Search(ModelSearch model, out List<CategoryViewModel> data)
        {
            CategoryViewModel item = new CategoryViewModel();
            List<CategoryViewModel> list = new List<CategoryViewModel>();
            data = new List<CategoryViewModel>();
            int results=0;
            data = new List<CategoryViewModel>();
            try
            {
                string strQuery = "SELECT cate.`id`, cate.`code`, cate.`name`, cate.`description`,cate_parent.`id` as parent_id,cate_parent.`name` as parent_name";
                strQuery += " FROM product_category cate LEFT JOIN(SELECT `id`, `name` FROM `product_category`) cate_parent";
                strQuery += " ON cate.`parent_id` = cate_parent.`id`";
                //string strQuery = "SELECT * FROM `product_category`";
                data = _db.Query<CategoryViewModel>(strQuery, new { parent_id = model.parent_id }).ToList();

                if (!Utilities.isNullOrEmpty(model.parent_id))
                {
                    list = data.Where(x => x.parent_id == model.parent_id).ToList();
                    data = list;
                }
              if (model.Child_id!=(int)Common.ReturnCode.Succeed && !Utilities.isNullOrEmpty(model.Child_id))
                {
                    list= data.Where(x => x.id == model.Child_id).ToList();
                    data = list;
                }
               if (!Utilities.isNullOrEmpty(model.code))
                {
                    list = data.Where(x => x.code == model.code).ToList();
                    data = list;
                }
                if (!Utilities.isNullOrEmpty(model.name))
                {
                    list = data.Where(x => x.name == model.name).ToList();
                    data = list;
                }
                if (!Utilities.isNullOrEmpty(model.parent_id)&& Utilities.isNullOrEmpty(model.code)&& Utilities.isNullOrEmpty(model.name))
                {
                    strQuery += "   WHERE  cate_parent.`id` = @id";
                    var itemparent = _db.Query<CategoryViewModel>(strQuery, new { id = model.parent_id }).FirstOrDefault();
                    list.Add(itemparent);
                    data = list;
                }
                return results;
            }
            catch (Exception ex)
            {
                data = null;
            }
            return results;

        }
        public List<ComboboxParent> GetParent()
        {
            List<ComboboxParent> model = new List<ComboboxParent>();
            try
            {               
                string strQuery = "SELECT DISTINCT cate_parent.id AS id, cate_parent.name AS name";
                strQuery += "  FROM product_category cate INNER JOIN (SELECT id, name FROM product_category) cate_parent";
                strQuery += "  ON cate.parent_id = cate_parent.id";
                model= _db.Query<ComboboxParent>(strQuery).ToList();


                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<ComboboxParent> GetCategory(long id)
        {
            List<ComboboxParent> model = new List<ComboboxParent>();
            try
            {
                string strQuery = "SELECT DISTINCT id,name";
                strQuery += "  FROM product_category";
                strQuery += "  WHERE parent_id = @id";
                model = _db.Query<ComboboxParent>(strQuery,new { id = id }).ToList();
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
