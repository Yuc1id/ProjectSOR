using UnityEngine;
using System;

using Cysharp.Threading.Tasks;

using ProjectSOR;

public class Player_Weapon : MonoBehaviour
{
	public int attackMode = 1;
	private float damage = 0f;

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

	readonly string enemyTag = "Enemy";
	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision == null) return;

		if (collision.CompareTag(enemyTag))
		{
			var enemy = collision.GetComponent<Enemy_Base>();
			if (enemy != null)
			{
				enemy.ReceiveDamage(damage);
			}
		}
	}
}
