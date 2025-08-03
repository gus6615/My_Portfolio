
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace SESCO.InGame
{
    public enum PeerType
    {
        Up,
        Down,
        Left,
        Right
    }
    
    public sealed class PartContainer : UnitBase
    {
        private const float PeerDistance = 3.5f;
        public readonly Vector2Int[] PeerOffsets = new[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0)
        };
        
        public Rigidbody2D Rigid => rigid;
        [SerializeField] private List<Transform> partSlots;
        private List<PartBase> parts;
        public List<PartBase> Parts => parts;

        private Dictionary<PeerType, PartContainer> peers;
        public Vector2Int Position { get; set; }
        
        public override void Initialize(int id)
        {
            base.Initialize(id);

            parts = new();
            peers = new();

            animator.Play($"Part_{id}_Idle", 0, 0);
        }

        public bool IsFullSlot => parts.Count == 4;
        
        public void AddPart(PartBase part, int idx)
        {
            part.transform.SetParent(partSlots[idx]);
            part.transform.localPosition = Vector3.zero;
            parts.Add(part);
        }
        
        public void AddPeer(PartContainer peer, PeerType peerType)
        {
            if (!peers.TryAdd(peerType, peer))
                return;
            
            peer.transform.SetParent(this.transform);
            peer.transform.rotation = Quaternion.identity;
            rigid.mass = 1.0f;

            var offset = new Vector3(PeerOffsets[(int)peerType].x, PeerOffsets[(int)peerType].y, 0) * PeerDistance;
            peer.transform.localPosition = offset;
            
            var joint = peer.gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = rigid;
            joint.dampingRatio = 0.25f;
            joint.frequency = 0.5f;
        }

        public override void TakeDamage(int damage)
        {
            animator.Play($"Part_{ID}_Hit", 0, 0);
            InGameVFXManager.Instance.CreateVFX("Pref_Particle_HitParticle", this.transform.position);
            base.TakeDamage(damage);
        }

        public override void Die()
        {
            foreach (var part in Parts)
            {
                part.IsActive = false;
            }

            InGameVFXManager.Instance.CreateVFX("Pref_Particle_ExplosionBig", this.transform.position);
            animator.Play($"Part_{ID}_Dead", 0, 0);
        }
        
        public void OnEndAnimation()
        {
            animator.Play($"Part_{ID}_Idle", 0, 0);
        }
    }
}