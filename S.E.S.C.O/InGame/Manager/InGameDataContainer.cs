
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DevelopKit.BasicTemplate;
using UnityEngine;

namespace SESCO.InGame
{
    public class InGameDataContainer : Singleton<InGameDataContainer>
    {
        public List<UnitBase> Monsters { get; private set; }
        public List<PartContainer> PartContainer { get; private set; }
        public Dictionary<Vector2Int, PartContainer> PartContainerMap { get; private set; }
        public List<PartBase> Parts { get; private set; }
        public List<ProjectileBase> Projectiles { get; private set; }
        public Dictionary<string, List<ParticleSystem>> VFXPoolDic { get; private set; }
        public List<ParticleSystem> VFXList => VFXPoolDic.Values.SelectMany(x => x).ToList();

        public ObservableProperty<int> Exp = new(0);
        public ObservableProperty<int> Level = new(1);
        public int NeedExp => 5 + (Level.Value - 1) * 10;
        
        ~InGameDataContainer()
        {
            Exp.OnValueChanged -= CheckLevelUp;
        }
        
        public void Initialize()
        {
            Monsters = new List<UnitBase>();
            PartContainer = new List<PartContainer>();
            PartContainerMap = new Dictionary<Vector2Int, PartContainer>();
            Parts = new List<PartBase>();
            Projectiles = new List<ProjectileBase>();
            VFXPoolDic = new Dictionary<string, List<ParticleSystem>>();
            
            Exp.OnValueChanged += CheckLevelUp;
        }

        private void CheckLevelUp(int exp)
        {
            if (exp < NeedExp) return;
            
            Exp.Value = exp - NeedExp;
            Level.Value++;
            
            ManagerHub.UI.ShowUIAsync<InGameSelectorPopup>("UI/InGameSelectorPopup").Forget();
        }
    }
}