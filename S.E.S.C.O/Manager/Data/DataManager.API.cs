
using System.Collections.Generic;
using System.Linq;
using SESCO.SO;

namespace DevelopKit.BasicTemplate
{
    public partial class DataManager
    {
        public IReadOnlyList<UnitDataSO> UnitDataSOList => unitDataSoDic.Values.ToList();
        public IReadOnlyList<PartDataSO> PartDataSOList => partDataSoDic.Values.ToList();
        public IReadOnlyList<ProjectileDataSO> ProjectileDataSOList => projectileDataSoDic.Values.ToList();
        
        public UnitDataSO GetUnitDataSO(int id) => unitDataSoDic[id];
        public PartDataSO GetPartDataSO(int id) => partDataSoDic[id];
        public ProjectileDataSO GetProjectileDataSO(int id) => projectileDataSoDic[id];
    }
}