using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using FX.Core;
using FX.Utils.MvcPaging;
using System.Collections.Generic;
using System.Linq;

namespace EInvoice.CAdmin.Models
{
    public class MenusModels
    {
        public IList<Menu> RootMenus { get; set; }
        public IPagedList<MenuModel> PagedListMenus { get; set; }
    }

    public class MenuModel
    {
        public Menu Item { get; set; }
        public int Id { get; set; }
        public int Level { get; set; }
        public List<MenuModel> Items { get; set; }

        public MenuModel()
        {
            Items = new List<MenuModel>();
        }

        public MenuModel(Menu item, int level = 0)
            : this()
        {
            Item = item;
            Id = item.Id;
            Level = level;
        }

        public static IList<Menu> GetRoots(int comId, int position)
        {
            IMenusService menuSrv = IoC.Resolve<IMenusService>();
            IList<Menu> menus = menuSrv.GetRoots(comId, position);
            return menus;
        }

        public static IList<MenuModel> GetTree(int comId, int rootId)
        {
            List<MenuModel> MenuTrees = new List<MenuModel>();
            IMenusService menuSrv = IoC.Resolve<IMenusService>();
            IList<Menu> menus = menuSrv.GetList(comId);
            var baseMenu = new MenuModel(menuSrv.Getbykey(rootId));
            MenuTrees.Add(baseMenu);
            foreach (var item in menus.Where(x => x.ParentId == rootId))
            {
                var root = new MenuModel(item, 1);
                MenuTrees.Add(root);
                BuildMenuTree(menus, root);
            }
            return MenuTrees;
        }

        private static void BuildMenuTree(IList<Menu> items, MenuModel model)
        {
            foreach (var item in items.Where(x => x.ParentId == model.Id))
            {
                var level = model.Level + 1;
                var child = new MenuModel(item, level);
                model.Items.Add(child);
                BuildMenuTree(items, child);
            }
        }
    }
}