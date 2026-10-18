using System;
using UnityEngine;

[CreateAssetMenu()]
public class LevelSO : ScriptableObject {

    [Serializable]
    public struct StartingStockEntry {
        public KitchenObjectSO kitchenObjectSO;
        public int amount;
    }

    public int levelSerial;
    public string levelName;
    public Vector2 mapCoords;

    public int deliverCount;
    public float gameplayTime;

    [Header("Starting Stock")]
    public StartingStockEntry[] startingStockArray;

    [Header("Counter Layout (15 slots)")]
    public CounterType slot1;
    public CounterType slot2;
    public CounterType slot3;
    public CounterType slot4;
    public CounterType slot5;
    public CounterType slot6;
    public CounterType slot7;
    public CounterType slot8;
    public CounterType slot9;
    public CounterType slot10;
    public CounterType slot11;
    public CounterType slot12;
    public CounterType slot13;
    public CounterType slot14;
    public CounterType slot15;

    public CounterType[] GetSlots() {
        return new CounterType[] {
            slot1, slot2, slot3, slot4, slot5,
            slot6, slot7, slot8, slot9, slot10,
            slot11, slot12, slot13, slot14, slot15
        };
    }
}