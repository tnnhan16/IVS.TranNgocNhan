using IVS.DAL.DAO;
using IVS.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace IVS.BL
{
    public interface ICategoryBL
    {
        List<CategoryViewModel> GetAll();
        List<CategoryParent> GetListParent();
        int GetByID(long ID, out CategoryModel model);
        int GetDetail(long ID, out CategoryViewModel model);
        int Insert(CategoryModel model, out List<string> lstMsg);
        int Update(CategoryModel model, out List<string> lstMsg);
        int Delete(string id, out List<string> lstMsg);
        List<ComboboxParent> GetParent();
        List<ComboboxParent> GetCategory(long id);
        int Search(ModelSearch model,out List<CategoryViewModel> data);
    }

    public class CategoryBL : ICategoryBL
    {
        public CategoryDAO _categoryDAO;
        public CategoryBL()
        {
            _categoryDAO = new CategoryDAO();
        }

        public int Delete(string id, out List<string> lstMsg)
        {
            return _categoryDAO.Delete(int.Parse(id), out lstMsg);
        }

        public List<CategoryViewModel> GetAll()
        {
            return _categoryDAO.GetAll();
        }
        public int GetByID(long ID, out CategoryModel model)
        {
            return _categoryDAO.GetByID(ID, out model);
        }
        public int GetDetail(long ID, out CategoryViewModel model)
        {
            return _categoryDAO.GetDetail(ID, out model);
        }

        public List<CategoryParent> GetListParent()
        {
            return _categoryDAO.GetListParent();
        }

        public int Insert(CategoryModel model, out List<string> lstMsg)
        {
            return _categoryDAO.Insert(model, out lstMsg);
        }

        public int Update(CategoryModel model, out List<string> lstMsg)
        {
            return _categoryDAO.Update(model, out lstMsg);
        }
        public List<ComboboxParent> GetParent()
        {
            return _categoryDAO.GetParent();
        }
        public List<ComboboxParent> GetCategory(long id)
        {
            return _categoryDAO.GetCategory(id);
        }
        public int  Search(ModelSearch model,out List<CategoryViewModel> data)
        {
            return _categoryDAO.Search(model,out data);

        }
    }
}

