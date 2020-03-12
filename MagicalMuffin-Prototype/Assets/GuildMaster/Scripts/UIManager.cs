using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// TODO: Split UI Manager into smaller UI sections for each panel. This is getting too big.
public class UIManager : MonoBehaviour
{
    public GameObject relic_popup;
    public GameObject[] button_images;

    Queue<GameObject> popup_queue;
    GameObject current_popup = null;

    void Awake()
    {
        popup_queue = new Queue<GameObject>();
    }

    internal void CreateRelicPopup(AttackRelic relic, Relic_Type relic_type)
    {
        GameObject popup = Instantiate(relic_popup, this.transform);

        popup.transform.GetChild(0).GetComponentInChildren<Text>().text = relic.relic_name;
        popup.transform.GetChild(2).GetComponent<Text>().text = relic.description;

        foreach (char letter in relic.attack_name)
        {
            GameObject new_button;
            if (letter == 'x')
                new_button = Instantiate(button_images[0], popup.transform.GetChild(1));
            else if (letter == 'y')
                new_button = Instantiate(button_images[1], popup.transform.GetChild(1));
        }

        //popup.GetComponent<RectTransform>().localPosition = Vector3.zero;
        popup_queue.Enqueue(popup);
        //popup.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(OnPopUpClose);

        CheckPopups();
    }

    private void CheckPopups()
    {
        if (current_popup)
            return;

        if(popup_queue.Count > 0)
        {
            current_popup = popup_queue.Dequeue();
            current_popup.SetActive(true);
            Invoke("OnPopUpClose", 4);
        }
    }

    public void OnPopUpClose()
    {
        Destroy(current_popup);
        current_popup = null;
        CheckPopups();
    }
}
