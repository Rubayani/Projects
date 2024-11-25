using UnityEngine;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager instance;
    private void Awake() => instance = this;

    public ItemToolTip itemToolTip;
    public MonsterToolTip monsterToolTip;







}
