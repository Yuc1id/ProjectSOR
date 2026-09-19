using UnityEngine;

using ProjectSOR;

public class Enemy_Dummy : Enemy_Base
{
	[SerializeField] bool resetHPOnUpdate = true;

	protected override void Update()
	{
		if (resetHPOnUpdate && hp != maxHP)
		{
			hp = maxHP;
			Debug.Log($"{gameObject.name} HP reset to maxHP: {maxHP}");
		}
		else if (hp <= 0)
		{
			Defeat();
		}
	}
}
