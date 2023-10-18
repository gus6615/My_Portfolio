using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    public Transform objectTr, dungeon_0_objectTr, dungeon_1_objectTr, dungeon_1_room_objectTr, objectUI;

    public GameObject jem; // 0
    public GameObject reinforceOre; // 1
    public GameObject normalBox, specialBox; // 2, 3
    public GameObject item; // 4
    public GameObject NPC; // 5
    public GameObject soilSlime; // 6
    public GameObject ADSlime; // 7
    public GameObject ADSlime_Attack; // 8
    public GameObject SpeedSlime; // 9
    public GameObject bigSlime; // 10
    public GameObject mushroomSlime; // 11
    public GameObject eventBlock; // 12
    public GameObject damageText; // 13
    public GameObject damageSlide; // 14
    public GameObject dropPet; // 제외
    public GameObject reinforceTree; // 15
    public GameObject kingSlime; // 16
    public GameObject kingSlime_Attack; // 17
    public GameObject golem; // 18
    public GameObject manaOre; // 19
    public GameObject card; // 20
    public GameObject batSlime; // 21
    public GameObject D_1_holeSlime; // 22
    public GameObject D_1_mimicSlime; // 23
    public GameObject D_1_box; // 24
    public GameObject ancientTresure; // 25
    public GameObject D_1_Torch; // 26
    public GameObject D_0_Torch; // 27
    public GameObject D_1_Midboss; // 28
    public GameObject D_1_Midboss_Skill0; // 29
    public GameObject D_1_Midboss_Skill1; // 30
    public GameObject cashOre; // 31
    public GameObject D_1_boss; // 32
    public GameObject growthOre; // 33

    Queue<SoilSlime> soilSlime_queue = new Queue<SoilSlime>();
    Queue<ADSlime> adSlime_queue = new Queue<ADSlime>();
    Queue<ADSlimeAttack> adSlimeAttack_queue = new Queue<ADSlimeAttack>();
    Queue<SpeedSlime> speedSlime_queue = new Queue<SpeedSlime>();
    Queue<BigSlime> bigSlime_queue = new Queue<BigSlime>();
    Queue<MushroomSlime> mushroomSlime_queue = new Queue<MushroomSlime>();
    Queue<JemObject> jem_queue = new Queue<JemObject>();
    Queue<ReinforceOre> reinforceOre_queue = new Queue<ReinforceOre>();
    Queue<NormalBox> normalBox_queue = new Queue<NormalBox>();
    Queue<RuinBox> specialBox_queue = new Queue<RuinBox>();
    Queue<ItemObject> item_queue = new Queue<ItemObject>();
    Queue<Dungeon_0_NPC> dungeon_0_NPC_queue = new Queue<Dungeon_0_NPC>();
    Queue<EventBlock> eventBlock_queue = new Queue<EventBlock>();
    Queue<DamageText> damageText_queue = new Queue<DamageText>();
    Queue<DamageSlide> damageSlide_queue = new Queue<DamageSlide>();
    Queue<ReinforceTree> reinforceTree_queue = new Queue<ReinforceTree>();
    Queue<KingSlime> kingSlime_queue = new Queue<KingSlime>();
    Queue<KingSlime_Attack> kingSlimeAttack_queue = new Queue<KingSlime_Attack>();
    Queue<Golem> golem_queue = new Queue<Golem>();
    Queue<ManaOre> manaOre_queue = new Queue<ManaOre>();
    Queue<CardObject> card_queue = new Queue<CardObject>();
    Queue<BatSlime> batSlime_queue = new Queue<BatSlime>();
    Queue<D_1_HoleSlime> D_1_holeSlime_queue = new Queue<D_1_HoleSlime>();
    Queue<D_1_MimicSlime> D_1_mimicSlime_queue = new Queue<D_1_MimicSlime>();
    Queue<AncientBox> D_1_box_queue = new Queue<AncientBox>();
    Queue<AncientTresure> ancientTresure_queue = new Queue<AncientTresure>();
    Queue<D_1_Torch> D_1_Torch_queue = new Queue<D_1_Torch>();
    Queue<D_0_Torch> D_0_Torch_queue = new Queue<D_0_Torch>();
    Queue<D_1_Midboss> D_1_Midboss_queue = new Queue<D_1_Midboss>();
    Queue<D_1_Midboss_Skill0> D_1_Midboss_Skills_queue = new Queue<D_1_Midboss_Skill0>();
    Queue<D_1_Midboss_Skill1> D_1_Midboss_Skill1s_queue = new Queue<D_1_Midboss_Skill1>();
    Queue<CashOre> cashOres_queue = new Queue<CashOre>();
    Queue<D_1_Boss> D_1_boss_queue = new Queue<D_1_Boss>();
    Queue<GrowthOre> growthOre_queue = new Queue<GrowthOre>();

    private void Start()
    {
        instance = this;
    }

    private T CreateNewObject<T>(int type, Transform tr, Vector3 pos) where T : MonoBehaviour
    {
        T newObj = null;

        switch (type)
        {
            case 0: newObj = Instantiate(jem, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 1: newObj = Instantiate(reinforceOre, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 2: newObj = Instantiate(normalBox, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 3: newObj = Instantiate(specialBox, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 4: newObj = Instantiate(item, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 5: newObj = Instantiate(NPC, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 6: newObj = Instantiate(soilSlime, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 7: newObj = Instantiate(ADSlime, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 8: newObj = Instantiate(ADSlime_Attack, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 9: newObj = Instantiate(SpeedSlime, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 10: newObj = Instantiate(bigSlime, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 11: newObj = Instantiate(mushroomSlime, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 12: newObj = Instantiate(eventBlock, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 13: newObj = Instantiate(damageText, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 14: newObj = Instantiate(damageSlide, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 15: newObj = Instantiate(reinforceTree, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 16: newObj = Instantiate(kingSlime, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 17: newObj = Instantiate(kingSlime_Attack, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 18: newObj = Instantiate(golem, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 19: newObj = Instantiate(manaOre, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 20: newObj = Instantiate(card, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 21: newObj = Instantiate(batSlime, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 22: newObj = Instantiate(D_1_holeSlime, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 23: newObj = Instantiate(D_1_mimicSlime, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 24: newObj = Instantiate(D_1_box, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 25: newObj = Instantiate(ancientTresure, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 26: newObj = Instantiate(D_1_Torch, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 27: newObj = Instantiate(D_0_Torch, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 28: newObj = Instantiate(D_1_Midboss, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 29: newObj = Instantiate(D_1_Midboss_Skill0, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 30: newObj = Instantiate(D_1_Midboss_Skill1, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 31: newObj = Instantiate(cashOre, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 32: newObj = Instantiate(D_1_boss, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case 33: newObj = Instantiate(growthOre, pos, Quaternion.identity, tr).GetComponent<T>(); break;
        }

        newObj.gameObject.SetActive(false);
        return newObj;
    }

    public static T GetObject<T>(int type, Transform tr, Vector3 pos) where T : MonoBehaviour
    {
        int count = GetCount(type);
        if (count > 0)
        {
            T obj = null;

            switch (type)
            {
                case 0: obj = instance.jem_queue.Dequeue().GetComponent<T>(); break;
                case 1: obj = instance.reinforceOre_queue.Dequeue().GetComponent<T>(); break;
                case 2: obj = instance.normalBox_queue.Dequeue().GetComponent<T>(); break;
                case 3: obj = instance.specialBox_queue.Dequeue().GetComponent<T>(); break;
                case 4: obj = instance.item_queue.Dequeue().GetComponent<T>(); break;
                case 5: obj = instance.dungeon_0_NPC_queue.Dequeue().GetComponent<T>(); break;
                case 6: obj = instance.soilSlime_queue.Dequeue().GetComponent<T>(); break;
                case 7: obj = instance.adSlime_queue.Dequeue().GetComponent<T>(); break;
                case 8: obj = instance.adSlimeAttack_queue.Dequeue().GetComponent<T>(); break;
                case 9: obj = instance.speedSlime_queue.Dequeue().GetComponent<T>(); break;
                case 10: obj = instance.bigSlime_queue.Dequeue().GetComponent<T>(); break;
                case 11: obj = instance.mushroomSlime_queue.Dequeue().GetComponent<T>(); break;
                case 12: obj = instance.eventBlock_queue.Dequeue().GetComponent<T>(); break;
                case 13: obj = instance.damageText_queue.Dequeue().GetComponent<T>(); break;
                case 14: obj = instance.damageSlide_queue.Dequeue().GetComponent<T>(); break;
                case 15: obj = instance.reinforceTree_queue.Dequeue().GetComponent<T>(); break;
                case 16: obj = instance.kingSlime_queue.Dequeue().GetComponent<T>(); break;
                case 17: obj = instance.kingSlimeAttack_queue.Dequeue().GetComponent<T>(); break;
                case 18: obj = instance.golem_queue.Dequeue().GetComponent<T>(); break;
                case 19: obj = instance.manaOre_queue.Dequeue().GetComponent<T>(); break;
                case 20: obj = instance.card_queue.Dequeue().GetComponent<T>(); break;
                case 21: obj = instance.batSlime_queue.Dequeue().GetComponent<T>(); break;
                case 22: obj = instance.D_1_holeSlime_queue.Dequeue().GetComponent<T>(); break;
                case 23: obj = instance.D_1_mimicSlime_queue.Dequeue().GetComponent<T>(); break;
                case 24: obj = instance.D_1_box_queue.Dequeue().GetComponent<T>(); break;
                case 25: obj = instance.ancientTresure_queue.Dequeue().GetComponent<T>(); break;
                case 26: obj = instance.D_1_Torch_queue.Dequeue().GetComponent<T>(); break;
                case 27: obj = instance.D_0_Torch_queue.Dequeue().GetComponent<T>(); break;
                case 28: obj = instance.D_1_Midboss_queue.Dequeue().GetComponent<T>(); break;
                case 29: obj = instance.D_1_Midboss_Skills_queue.Dequeue().GetComponent<T>(); break;
                case 30: obj = instance.D_1_Midboss_Skill1s_queue.Dequeue().GetComponent<T>(); break;
                case 31: obj = instance.cashOres_queue.Dequeue().GetComponent<T>(); break;
                case 32: obj = instance.D_1_boss_queue.Dequeue().GetComponent<T>(); break;
                case 33: obj = instance.growthOre_queue.Dequeue().GetComponent<T>(); break;
            }

            obj.transform.SetParent(tr);
            obj.transform.position = pos;
            obj.gameObject.SetActive(true);

            return obj;
        }
        else
        {
            var newObj = instance.CreateNewObject<T>(type, tr, pos);
            newObj.transform.SetParent(tr);
            newObj.transform.position = pos;
            newObj.gameObject.SetActive(true);
            return newObj;
        }
    }

    public static void ReturnObject<T>(int type, T obj) where T : MonoBehaviour
    {
        if (obj == null)
        {
            Debug.LogError("Return Object is Failed.");
            return;
        }
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(instance.transform);

        switch (type)
        {
            case 0: instance.jem_queue.Enqueue(obj.GetComponent<JemObject>());  break;
            case 1: instance.reinforceOre_queue.Enqueue(obj.GetComponent<ReinforceOre>()); break;
            case 2: instance.normalBox_queue.Enqueue(obj.GetComponent<NormalBox>()); break;
            case 3: instance.specialBox_queue.Enqueue(obj.GetComponent<RuinBox>()); break;
            case 4: instance.item_queue.Enqueue(obj.GetComponent<ItemObject>()); break;
            case 5: instance.dungeon_0_NPC_queue.Enqueue(obj.GetComponent<Dungeon_0_NPC>()); break;
            case 6: instance.soilSlime_queue.Enqueue(obj.GetComponent<SoilSlime>()); break;
            case 7: instance.adSlime_queue.Enqueue(obj.GetComponent<ADSlime>()); break;
            case 8: instance.adSlimeAttack_queue.Enqueue(obj.GetComponent<ADSlimeAttack>()); break;
            case 9: instance.speedSlime_queue.Enqueue(obj.GetComponent<SpeedSlime>()); break;
            case 10: instance.bigSlime_queue.Enqueue(obj.GetComponent<BigSlime>()); break;
            case 11: instance.mushroomSlime_queue.Enqueue(obj.GetComponent<MushroomSlime>()); break;
            case 12: instance.eventBlock_queue.Enqueue(obj.GetComponent<EventBlock>()); break;
            case 13: instance.damageText_queue.Enqueue(obj.GetComponent<DamageText>()); break;
            case 14: instance.damageSlide_queue.Enqueue(obj.GetComponent<DamageSlide>()); break;
            case 15: instance.reinforceTree_queue.Enqueue(obj.GetComponent<ReinforceTree>()); break;
            case 16: instance.kingSlime_queue.Enqueue(obj.GetComponent<KingSlime>()); break;
            case 17: instance.kingSlimeAttack_queue.Enqueue(obj.GetComponent<KingSlime_Attack>()); break;
            case 18: instance.golem_queue.Enqueue(obj.GetComponent<Golem>()); break;
            case 19: instance.manaOre_queue.Enqueue(obj.GetComponent<ManaOre>()); break;
            case 20: instance.card_queue.Enqueue(obj.GetComponent<CardObject>()); break;
            case 21: instance.batSlime_queue.Enqueue(obj.GetComponent<BatSlime>()); break;
            case 22: instance.D_1_holeSlime_queue.Enqueue(obj.GetComponent<D_1_HoleSlime>()); break;
            case 23: instance.D_1_mimicSlime_queue.Enqueue(obj.GetComponent<D_1_MimicSlime>()); break;
            case 24: instance.D_1_box_queue.Enqueue(obj.GetComponent<AncientBox>()); break;
            case 25: instance.ancientTresure_queue.Enqueue(obj.GetComponent<AncientTresure>()); break;
            case 26: instance.D_1_Torch_queue.Enqueue(obj.GetComponent<D_1_Torch>()); break;
            case 27: instance.D_0_Torch_queue.Enqueue(obj.GetComponent<D_0_Torch>()); break;
            case 28: instance.D_1_Midboss_queue.Enqueue(obj.GetComponent<D_1_Midboss>()); break;
            case 29: instance.D_1_Midboss_Skills_queue.Enqueue(obj.GetComponent<D_1_Midboss_Skill0>()); break;
            case 30: instance.D_1_Midboss_Skill1s_queue.Enqueue(obj.GetComponent<D_1_Midboss_Skill1>()); break;
            case 31: instance.cashOres_queue.Enqueue(obj.GetComponent<CashOre>()); break;
            case 32: instance.D_1_boss_queue.Enqueue(obj.GetComponent<D_1_Boss>()); break;
            case 33: instance.growthOre_queue.Enqueue(obj.GetComponent<GrowthOre>()); break;
        }
    }

    private static int GetCount(int type)
    {
        int count = 0;

        switch (type)
        {
            case 0: count = instance.jem_queue.Count; break;
            case 1: count = instance.reinforceOre_queue.Count; break;
            case 2: count = instance.normalBox_queue.Count; break;
            case 3: count = instance.specialBox_queue.Count; break;
            case 4: count = instance.item_queue.Count; break;
            case 5: count = instance.dungeon_0_NPC_queue.Count; break;
            case 6: count = instance.soilSlime_queue.Count; break;
            case 7: count = instance.adSlime_queue.Count; break;
            case 8: count = instance.adSlimeAttack_queue.Count; break;
            case 9: count = instance.speedSlime_queue.Count; break;
            case 10: count = instance.bigSlime_queue.Count; break;
            case 11: count = instance.mushroomSlime_queue.Count; break;
            case 12: count = instance.eventBlock_queue.Count; break;
            case 13: count = instance.damageText_queue.Count; break;
            case 14: count = instance.damageSlide_queue.Count; break;
            case 15: count = instance.reinforceTree_queue.Count; break;
            case 16: count = instance.kingSlime_queue.Count; break;
            case 17: count = instance.kingSlimeAttack_queue.Count; break;
            case 18: count = instance.golem_queue.Count; break;
            case 19: count = instance.manaOre_queue.Count; break;
            case 20: count = instance.card_queue.Count; break;
            case 21: count = instance.batSlime_queue.Count; break;
            case 22: count = instance.D_1_holeSlime_queue.Count; break;
            case 23: count = instance.D_1_mimicSlime_queue.Count; break;
            case 24: count = instance.D_1_box_queue.Count; break;
            case 25: count = instance.ancientTresure_queue.Count; break;
            case 26: count = instance.D_1_Torch_queue.Count; break;
            case 27: count = instance.D_0_Torch_queue.Count; break;
            case 28: count = instance.D_1_Midboss_queue.Count; break;
            case 29: count = instance.D_1_Midboss_Skills_queue.Count; break;
            case 30: count = instance.D_1_Midboss_Skill1s_queue.Count; break;
            case 31: count = instance.cashOres_queue.Count; break;
            case 32: count = instance.D_1_boss_queue.Count; break;
            case 33: count = instance.growthOre_queue.Count; break;
        }

        return count;
    }
}
