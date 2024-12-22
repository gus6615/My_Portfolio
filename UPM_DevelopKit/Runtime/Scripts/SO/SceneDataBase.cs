using System;
using System.Collections.Generic;
using UnityEngine;

namespace DevelopKit
{
    [CreateAssetMenu(fileName = "SceneDataBase", menuName = "DevelopKit/SO/SceneDataBase", order = 1)]
    public class SceneDataBase : ScriptableObject
    {
        [SerializeField] private List<SceneData> _sceneDatas;
        public List<SceneData> SceneDatas => _sceneDatas;
    }
}