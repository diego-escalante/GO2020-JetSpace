using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour {
    
    public GameObject coinTrackerPrefab;

    private Transform uiList;
    private Dictionary<Color, CoinType> coinMap = new Dictionary<Color, CoinType>();

    private void Start() {
        uiList = GameObject.FindGameObjectWithTag("UI").transform.Find("UIList");
    }

    public void AddCoin(Color c) {
        if (!coinMap.ContainsKey(c)) {
            coinMap.Add(c, new CoinType(c, uiList, coinTrackerPrefab));    
        }
        coinMap[c].AddCoin();
    }

    public bool hasCoins(Color color, int amount) {
        if (!coinMap.ContainsKey(color)) {
            return false;
        }
        return coinMap[color].GetCoinsInPocket() >= amount;
    }

    public void payCoins(Color color, int amount) {
        if (!coinMap.ContainsKey(color)) {
            return;
        }
        coinMap[color].RemoveCoins(amount);
    }

    private class CoinType {
        private TMP_Text coinTracker;
        private int inPocket = 0;
        private int totalCollected = 0;
        private int totalExisting;

        public CoinType(Color c, Transform uiList, GameObject coinTrackerPrefab) {
            GameObject o = Instantiate(coinTrackerPrefab, Vector3.zero, Quaternion.identity, uiList);
            o.transform.SetAsFirstSibling();
            o.transform.Find("Image").GetComponent<Image>().color = c;
            coinTracker = o.transform.Find("Text").GetComponent<TMP_Text>();
            coinTracker.color = c;
        }

        public void AddCoin() {
            totalCollected++;
            inPocket++;
            coinTracker.text = inPocket.ToString();
        }

        public int GetCoinsInPocket() {
            return inPocket;
        }

        public void RemoveCoins(int amount) {
            inPocket = Mathf.Max(0, inPocket - amount);
            coinTracker.text = inPocket.ToString();
        }
    }

}
