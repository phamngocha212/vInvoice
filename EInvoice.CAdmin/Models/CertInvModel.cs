using EInvoice.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using FX.Context;
using FX.Core;
using FX.Utils.MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Models
{
    public class CertInvModel
    {
        public IList<string> Patterns { get; set; }
        public string pattern { get; set; }
        public int status { get; set; }
        public int type { get; set; }
        public IPagedList<InvCertify> PagedListInvCert { get; set; }
    }

    public class MenuModels
    {
        public MenuModels()
        {
            this.Items = new List<MenuModels>();
        }
        public MenuModels(Menu mMenu, int position, int level = 0) : this()
        {
            this.MenuBase = mMenu;
            this.Position = position;
            this.Level = level;
        }
        public Menu MenuBase { get; set; }
        public List<MenuModels> Items { get; set; }
        public int Level { get; set; }
        public int Position { get; set; }
        static IDictionary<string, IList<MenuModels>> MenuForCompany;

        public static IList<MenuModels> GetTree(int position)
        {
            IList<MenuModels> menuTrees = new List<MenuModels>();
            Company currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany; 
            string key = string.Format("{0}", currentCompany.TaxCode);
            if (MenuForCompany != null && MenuForCompany.Keys.Contains(key))
            {
                menuTrees = MenuForCompany[key] as IList<MenuModels>;
                return menuTrees.Where(p => p.Position == position).ToList();
            }                
            MenuForCompany = new Dictionary<string, IList<MenuModels>>();
            IMenusService menuSrv = IoC.Resolve<IMenusService>();
            IList<Menu> ListMenu = menuSrv.ListActived(currentCompany.id);
            
            foreach (Menu item in ListMenu.Where(p => p.ParentId == 0).OrderBy(p=>p.Priority))
            {
                var root = new MenuModels(item, item.Position);
                menuTrees.Add(root);
                BuildMenuTree(ListMenu, root);
            }
            MenuForCompany.Add(key, menuTrees);
            return menuTrees.Where(p=>p.Position == position).ToList();
        }

        public static void ResetMenu()
        {
            IList<MenuModels> menuTrees = new List<MenuModels>();           
            Company currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany; 
            string key = string.Format("{0}", currentCompany.TaxCode);
            MenuForCompany = new Dictionary<string, IList<MenuModels>>();
            IMenusService menuSrv = IoC.Resolve<IMenusService>();
            IList<Menu> ListMenu = menuSrv.ListActived(currentCompany.id);            
            foreach (Menu item in ListMenu.Where(p => p.ParentId == 0).OrderBy(p => p.Priority))
            {
                var root = new MenuModels(item, item.Position);
                menuTrees.Add(root);
                BuildMenuTree(ListMenu, root);
            }
            MenuForCompany.Add(key, menuTrees);
        }

        private static void BuildMenuTree(IList<Menu> items, MenuModels model)
        {
            foreach (var item in items.Where(x => x.ParentId == model.MenuBase.Id && x.Position == model.Position).OrderBy(p => p.Priority))
            {
                var level = model.Level + 1;
                var child = new MenuModels(item, model.Position, level);
                model.Items.Add(child);
                BuildMenuTree(items, child);
            }
        }
    }
}