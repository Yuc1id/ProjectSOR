using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class Player_Weapon : MonoBehaviour
{
	public int attackMode = 1;
	public float damage = 0f;

	[SerializeField] GameObject heavyBlowObject;
	[SerializeField] float heavyBlowDuration;
	[SerializeField] float heavyBlowCooldown;
	[SerializeField] float heavyBlowATK;

	bool canHeavyBlow = false;
	int existingHeavyBlowCount = 0;

	//Attacks

	//#1 ƒVƒƒƒR
	async UniTask HeavyBlowCooldown()
	{
		canHeavyBlow = true;
		await UniTask.Delay(TimeSpan.FromSeconds(heavyBlowCooldown));
		canHeavyBlow = false;
	}
	async UniTask HeavyBlow()
    {
		if (!canHeavyBlow)
		{
			_ = HeavyBlowCooldown();

			GameObject blowEffect = Instantiate(heavyBlowObject, transform);
			existingHeavyBlowCount++;
			damage = heavyBlowATK;

			await UniTask.Delay(TimeSpan.FromSeconds(heavyBlowDuration));
			Destroy(blowEffect);
			existingHeavyBlowCount--;

			if (existingHeavyBlowCount <= 0) damage = 0f;
		}
	}

	public UniTask Attack(int mode)
	{
		switch (mode)
		{
			case 1:
				return HeavyBlow();
			default:
				throw new ArgumentException("Invalid attack mode");
		}
	}
}
