
using System.Collections.Generic;
using SESCO.SO;
using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    public partial class DataManager
    {
        private readonly string unitDataKey = "unit";
        private readonly string partDataKey = "part";
        private readonly string projectileDataKey = "projectile";
        
        private readonly Dictionary<int, UnitDataSO> unitDataSoDic = new();
        private readonly Dictionary<int, PartDataSO> partDataSoDic = new(); 
        private readonly Dictionary<int, ProjectileDataSO> projectileDataSoDic = new();
        
        private static Sprite onAimMouseSprite;
        public static Sprite OnAimMouseSprite
        {
            get
            {
                if (onAimMouseSprite == null)
                {
                    onAimMouseSprite = AddressableUtil.LoadAsset<Sprite>("UI/OnAimMouse");
                }

                return onAimMouseSprite;
            }
        }
        
        private static Sprite normalMouseSprite;
        public static Sprite NormalMouseSprite
        {
            get
            {
                if (normalMouseSprite == null)
                {
                    normalMouseSprite = AddressableUtil.LoadAsset<Sprite>("UI/NormalMouse");
                }

                return normalMouseSprite;
            }
        }
        
        private static Sprite hoverMouseSprite;
        public static Sprite HoverMouseSprite
        {
            get
            {
                if (hoverMouseSprite == null)
                {
                    hoverMouseSprite = AddressableUtil.LoadAsset<Sprite>("UI/HoverMouse");
                }

                return hoverMouseSprite;
            }
        }
    }
}