using NetCorePal.D3Shop.Domain.AggregatesModel.Identity.RoleAggregate;
using NetCorePal.Extensions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCorePal.D3Shop.Domain.AggregatesModel.Identity.MenuAggregate
{

    public partial record MenuId : IInt64StronglyTypedId;

    /// <summary>
    /// 系统菜单实体类，定义了菜单的基本结构和层级关系
    /// </summary>
    public class Menu : Entity<MenuId>, IAggregateRoot
    {
        protected Menu()
        {
        }

        
    }
}
