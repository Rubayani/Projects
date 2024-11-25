using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTabsHandler : MonoBehaviour
{
	[SerializeField] private Transform tabsParent;
    
    public void Display(GameObject tab)
    {
        tab.transform.SetAsLastSibling();
    }
}
