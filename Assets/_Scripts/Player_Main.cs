using UnityEngine;

using Alchemy.Inspector;

public class Player_Main : MonoBehaviour
{
	//Constant
	[FoldoutGroup("CheckGround")][SerializeField] Player_CheckGround checkGround, checkHead, checkWallFront, checkWallBack; //接地判定用オブジェクト
	[FoldoutGroup("CheckGround")][SerializeField] Player_CheckGround checkWater; //水中判定用オブジェクト

	[TabGroup("Movement", "Walk")][SerializeField] float walkSpeed;
	[TabGroup("Movement", "Walk")][SerializeField] AnimationCurve walkCurve; //移動速度表現
	[TabGroup("Movement", "Jump")][SerializeField] float gravity; //下向き＋
	[TabGroup("Movement", "Jump")][SerializeField] float jumpSpeed;
	[TabGroup("Movement", "Jump")][SerializeField] AnimationCurve jumpCurve; //ジャンプ速度表現
	[TabGroup("Movement", "Jump")][SerializeField] float jumpLimitTime; //ジャンプ上限時間
	[TabGroup("Movement", "Jump")][SerializeField] float jumpMaxFallSpeed;
	[TabGroup("Movement", "Jump")][SerializeField] AnimationCurve objJumpCurve; //オブジェクトジャンプ表現
	[TabGroup("Movement", "Swim")][SerializeField] float swimSpeed;
	[TabGroup("Movement", "Swim")][SerializeField] float swimAcceleration;
	[TabGroup("Movement", "Swim")][SerializeField] float swimDeceleration; //ニュートラル時の減速率

	[SerializeField] Player_Weapon weapon; //武器オブジェクト

	//Objects
	InputSystem_Actions inputSys; //InputSystem Object
	Rigidbody2D rb = null; //physics
	Animator anim = null; //Animation

	//Input
	float walkKeyFloat;
	bool jumpKeyBool;
	Vector2 swimKeyVector2;
	bool attackKeyBool;

	//Script Variable
	bool isGround = false; //CheckGround under
	bool isHead = false; //CheckGround above
	bool isWallFront = false; //CheckGround front
	bool isWallBack = false; //CheckGround back
	bool isWater = false; //CheckGround water

	Vector2 beforeSpeed = Vector2.zero; //前フレームの速度

	float beforeWalkSpeed = 0f;
	float walkTime; //for AnimationCurve

	float beforeJumpSpeed = 0f;
	float jumpTime; //for AnimationCurve

	bool isJump = false; //プレイヤーがジャンプ（上昇）中か
	bool isObjectJump = false; //プレイヤーがオブジェクト（ばねなど）によるジャンプ（上昇）中か
	float objectJumpHeight = 0f; //オブジェクトジャンプの高度
	//Animation Variable
	float animWalkSpeed = 0.0f;
	bool animIsJump = false;
	bool animIsWallJump = false;

	public int attackMode = 1; //攻撃モード

	/// <summary>
	///	歩行速度の計算（-speed以上speed以下）
	/// </summary>
	/// <param name="hKey">横移動キー</param>
	/// <returns>歩行速度(float)</returns>
	Vector2 GetSpeed_Walk(float hKey)
	{
		const int turnDecelRate = 5; //ターン（キーを切り替えた）時の減速最大フレーム数
		const int stopDecelRate = 15; //停止（キーを離した）時の減速最大フレーム数

		float calcSpeed = beforeWalkSpeed;

		//(hKey = -1, 0, 1)
		float bws_sign = Mathf.Sign(beforeWalkSpeed);
		float hKey_sign = Mathf.Sign(hKey);

		if (hKey != 0) //キーが押された
		{
			transform.localScale = new Vector3(hKey_sign, 1, 1);
			//速度計算
			if (hKey * beforeWalkSpeed < 0) //ターン
			{
				calcSpeed += hKey * walkSpeed / turnDecelRate; //減速
				walkTime = 0.0f; //AnimationCurveを無視
				if (hKey * calcSpeed > 0) calcSpeed = 0.0f; //ターン終わり
			}
			else //直進(AnimationCurveにしたがって加速)
			{
				calcSpeed = hKey * walkSpeed * walkCurve.Evaluate(walkTime);
				walkTime += Time.deltaTime;
			}
		}
		else
		{
			calcSpeed -= bws_sign * walkSpeed / stopDecelRate; //減速
			walkTime = 0.0f; //AnimationCurveを無視
			if (bws_sign * calcSpeed < 0) calcSpeed = 0.0f; //減速終わり
		}
		if (isWallFront) //壁にめり込もうとする向き
		{
			calcSpeed = 0f;
			walkTime = 0.0f;
		}

		//最高速度(walkSpeed)に対する現在の速度(calcSpeed)の割合　アニメーション用
		animWalkSpeed = Mathf.Abs(calcSpeed / walkSpeed);
		//現フレームのステータスを保存
		beforeWalkSpeed = calcSpeed;

		return new Vector2(calcSpeed, 0);
	}

	Vector2 GetSpeed_Jump(bool jKey)
	{
		bool canTime = jumpLimitTime > jumpTime; //ジャンプが時間切れでないか
		float calcSpeed = beforeJumpSpeed;

		if (isHead) calcSpeed = 0.0f;

		if (isObjectJump)
		{
			if (canTime && !isHead) //上昇続行　AnimationCurve適用
			{
				calcSpeed = objectJumpHeight * objJumpCurve.Evaluate(jumpTime);
				jumpTime += Time.deltaTime;
			}
			else //落下開始　AnimationCurve無視
			{
				isObjectJump = false;
				jumpTime = 0.0f;
				calcSpeed = 0.0f;
			}
		}
		else if (isJump) //ジャンプ上昇中
		{

			if (jKey && canTime && !isHead) //上昇続行　AnimationCurve適用
			{
				jumpTime += Time.deltaTime;
				calcSpeed = jumpSpeed * jumpCurve.Evaluate(jumpTime);
			}
			else //落下開始　AnimationCurve無視
			{
				isJump = false;
				jumpTime = 0.0f;
				calcSpeed -= gravity; //慣性を残す
			}
		}
		else if (isGround && !isHead) //地上
		{
			if (jKey) //上昇開始　AnimationCurve適用
			{
				isJump = true;
				jumpTime = 0.0f;
				calcSpeed = jumpSpeed * jumpCurve.Evaluate(jumpTime);
			}
			else //地上
			{
				calcSpeed = -1f; //下向きの速度を与えて正確に接地させる
			}
		}
		else //落下続行　AnimationCurve無視
		{
			calcSpeed -= gravity;
			calcSpeed = Mathf.Clamp(calcSpeed, jumpMaxFallSpeed, float.MaxValue); //下限設定
		}
		//現フレームのステータスを保存
		beforeJumpSpeed = calcSpeed;

		animIsJump = isJump || calcSpeed > 0f;
		return new Vector2(0, calcSpeed);
	}

	Vector2 GetSpeed_Swim(Vector2 sKey)
	{
		Vector2 calcSpeed = beforeSpeed;

		float sKeyH_sign = Mathf.Sign(sKey.x);
		if (sKey.x != 0) transform.localScale = new Vector3(sKeyH_sign, 1, 1);

		if (isWallFront) calcSpeed.x = 0f;
		if (isGround || isHead) calcSpeed.y = 0f;

		if (sKey != Vector2.zero)
		{
			calcSpeed += sKey * swimAcceleration;
			calcSpeed.x = Mathf.Clamp(calcSpeed.x, -swimSpeed, swimSpeed);
			calcSpeed.y = Mathf.Clamp(calcSpeed.y, -swimSpeed, swimSpeed);
		}
		else
		{
			calcSpeed *= swimDeceleration;
		}

		beforeWalkSpeed = calcSpeed.x;
		beforeJumpSpeed = calcSpeed.y;
		return calcSpeed;
	}

	//InputSystem
	void OnEnable()
	{
		inputSys = new InputSystem_Actions();
		inputSys.Enable();
	}
	void OnDisable()
	{
		inputSys.Disable();
	}

	void Start()
	{
		//Load Object
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}

	void FixedUpdate()
	{
		//Load Input
		walkKeyFloat = inputSys.Player.MoveHorizontal.ReadValue<float>();
		jumpKeyBool = inputSys.Player.Jump.IsPressed();
		swimKeyVector2 = inputSys.Player.Move.ReadValue<Vector2>();
		attackKeyBool = inputSys.Player.Attack.IsPressed();

		isGround = checkGround.IsGround();
		isHead = checkHead.IsGround();
		isWallFront = checkWallFront.IsGround();
		isWallBack = checkWallBack.IsGround();
		isWater = checkWater.IsWater();

		//Move
		if (isWater)
		{
			rb.linearVelocity = GetSpeed_Swim(swimKeyVector2);
		}
		else
		{
			rb.linearVelocity = GetSpeed_Walk(walkKeyFloat) + GetSpeed_Jump(jumpKeyBool);
		}

		//Attack
		if (attackKeyBool)
		{
			if (attackMode == 1) weapon.Attack(1);
		}

		beforeSpeed = rb.linearVelocity;

		//Animation
		anim.SetFloat("WalkSpeed", animWalkSpeed, 0.1f, Time.deltaTime);
	}

}
