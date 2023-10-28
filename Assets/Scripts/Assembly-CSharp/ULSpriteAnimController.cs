using UnityEngine;

public class ULSpriteAnimController : ULAnimControllerInterface
{
	public bool animate;

	public int[] uvToVertMap;

	public ULSpriteAnimModelInterface animationModel;

	public ULSpriteAnimationSetting currentAnimationSetting = new ULSpriteAnimationSetting();

	public MeshFilter quad;

	private Vector2[] uvs = new Vector2[4];

	private float frame;

	private float elapsed;

	private float seconds_per_frame;

	private Material material;

	private Vector2[] uvOrder = new Vector2[4]
	{
		Vector2.zero,
		Vector2.zero,
		Vector2.zero,
		Vector2.zero
	};

	protected void StartAnim()
	{
		elapsed = 0f;
		SetupSprite();
		UpdateSprite();
	}

	public void Update()
	{
		elapsed += Time.deltaTime;
		if (animate && currentAnimationSetting.cellCount > 1)
		{
			UpdateSprite();
		}
	}

	private void SetupSprite()
	{
		ULSpriteAnimationSetting uLSpriteAnimationSetting = currentAnimationSetting;
		string materialName = animationModel.GetMaterialName(uLSpriteAnimationSetting.animName);
		if (materialName == null)
		{
			return;
		}
		material = null;
		if ((bool)material && (bool)quad)
		{
			MeshRenderer meshRenderer = (MeshRenderer)quad.GetComponent("MeshRenderer");
			if ((bool)meshRenderer)
			{
				meshRenderer.material = material;
			}
		}
	}

	private void UpdateSprite()
	{
		ULSpriteAnimationSetting uLSpriteAnimationSetting = currentAnimationSetting;
		seconds_per_frame = ((uLSpriteAnimationSetting.framesPerSecond != 0) ? (1f / (float)uLSpriteAnimationSetting.framesPerSecond) : 0f);
		if (uLSpriteAnimationSetting.timingTotal == 0f)
		{
			switch (uLSpriteAnimationSetting.loopMode)
			{
			case ULSpriteAnimationSetting.LoopMode.None:
				if (elapsed > (float)uLSpriteAnimationSetting.cellCount * seconds_per_frame)
				{
					elapsed = (float)uLSpriteAnimationSetting.cellCount * seconds_per_frame;
				}
				frame = elapsed / seconds_per_frame;
				break;
			case ULSpriteAnimationSetting.LoopMode.Loop:
				elapsed = ((!(seconds_per_frame > 0f)) ? 0f : Mathf.Repeat(elapsed, (float)uLSpriteAnimationSetting.cellCount * seconds_per_frame));
				frame = ((!(seconds_per_frame > 0f)) ? 0f : (elapsed / seconds_per_frame));
				break;
			}
		}
		else
		{
			switch (uLSpriteAnimationSetting.loopMode)
			{
			case ULSpriteAnimationSetting.LoopMode.None:
				if (elapsed > uLSpriteAnimationSetting.timingTotal)
				{
					elapsed = uLSpriteAnimationSetting.timingTotal;
				}
				break;
			case ULSpriteAnimationSetting.LoopMode.Loop:
				elapsed = Mathf.Repeat(elapsed, uLSpriteAnimationSetting.timingTotal);
				break;
			}
			frame = 0f;
			foreach (float timing in uLSpriteAnimationSetting.timingList)
			{
				float num = timing;
				if (elapsed <= num)
				{
					break;
				}
				frame += 1f;
			}
		}
		if (frame < 0f)
		{
			frame = 0f;
		}
		if (frame > (float)(uLSpriteAnimationSetting.cellCount - 1))
		{
			frame = uLSpriteAnimationSetting.cellCount - 1;
		}
		int num2 = (int)((frame + (float)uLSpriteAnimationSetting.cellStartColumn) % (float)uLSpriteAnimationSetting.cellColumns);
		int num3 = (int)((frame + (float)uLSpriteAnimationSetting.cellStartColumn) / (float)uLSpriteAnimationSetting.cellColumns);
		float num4 = uLSpriteAnimationSetting.cellWidth * (float)num2 + uLSpriteAnimationSetting.cellLeft;
		float num5 = 1f - (uLSpriteAnimationSetting.cellHeight * (float)num3 + uLSpriteAnimationSetting.cellTop);
		float new_x = (uLSpriteAnimationSetting.flipH ? (num4 + uLSpriteAnimationSetting.cellWidth) : num4);
		float new_x2 = (uLSpriteAnimationSetting.flipH ? num4 : (num4 + uLSpriteAnimationSetting.cellWidth));
		float new_y = (uLSpriteAnimationSetting.flipV ? (num5 - uLSpriteAnimationSetting.cellHeight) : num5);
		float new_y2 = (uLSpriteAnimationSetting.flipV ? num5 : (num5 - uLSpriteAnimationSetting.cellHeight));
		if (uLSpriteAnimationSetting.texture != null)
		{
		}
		uvOrder[0].Set(new_x, new_y);
		uvOrder[1].Set(new_x2, new_y);
		uvOrder[2].Set(new_x, new_y2);
		uvOrder[3].Set(new_x2, new_y2);
		if (uvToVertMap != null && uvToVertMap.Length == 4)
		{
			uvs[0] = uvOrder[uvToVertMap[0]];
			uvs[1] = uvOrder[uvToVertMap[1]];
			uvs[2] = uvOrder[uvToVertMap[2]];
			uvs[3] = uvOrder[uvToVertMap[3]];
		}
		else
		{
			uvs[0] = uvOrder[0];
			uvs[1] = uvOrder[1];
			uvs[2] = uvOrder[2];
			uvs[3] = uvOrder[3];
		}
		if ((bool)quad)
		{
			Mesh mesh = quad.mesh;
			if (mesh != null)
			{
				mesh.uv = uvs;
			}
		}
	}

	public bool HasAnimation(string animationName)
	{
		return animationModel.HasAnimation(animationName);
	}

	public bool AnimationEnabled()
	{
		return animate;
	}

	public void EnableAnimation(bool enabled)
	{
		animate = enabled;
	}

	private void ApplyAnimation(string animationName)
	{
		ULSpriteAnimationSetting uLSpriteAnimationSetting = currentAnimationSetting;
		ULSpriteAnimModelInterface uLSpriteAnimModelInterface = animationModel;
		uLSpriteAnimationSetting.animName = animationName;
		uLSpriteAnimationSetting.resourceName = uLSpriteAnimModelInterface.GetResourceName(animationName);
		uLSpriteAnimationSetting.texture = uLSpriteAnimModelInterface.GetTextureName(animationName);
		uLSpriteAnimationSetting.cellTop = uLSpriteAnimModelInterface.CellTop(animationName);
		uLSpriteAnimationSetting.cellLeft = uLSpriteAnimModelInterface.CellLeft(animationName);
		uLSpriteAnimationSetting.cellWidth = uLSpriteAnimModelInterface.CellWidth(animationName);
		uLSpriteAnimationSetting.cellHeight = uLSpriteAnimModelInterface.CellHeight(animationName);
		uLSpriteAnimationSetting.cellStartColumn = uLSpriteAnimModelInterface.CellStartColumn(animationName);
		uLSpriteAnimationSetting.cellColumns = uLSpriteAnimModelInterface.CellColumns(animationName);
		uLSpriteAnimationSetting.cellCount = uLSpriteAnimModelInterface.CellCount(animationName);
		uLSpriteAnimationSetting.framesPerSecond = uLSpriteAnimModelInterface.FramesPerSecond(animationName);
		uLSpriteAnimationSetting.timingTotal = uLSpriteAnimModelInterface.TimingTotal(animationName);
		uLSpriteAnimationSetting.timingList = uLSpriteAnimModelInterface.TimingList(animationName);
		uLSpriteAnimationSetting.loopMode = (uLSpriteAnimModelInterface.Loop(animationName) ? ULSpriteAnimationSetting.LoopMode.Loop : ULSpriteAnimationSetting.LoopMode.None);
		uLSpriteAnimationSetting.flipH = uLSpriteAnimModelInterface.FlipH(animationName);
		uLSpriteAnimationSetting.flipV = uLSpriteAnimModelInterface.FlipV(animationName);
	}

	public void PlayAnimation(string animationName)
	{
		if (HasAnimation(animationName))
		{
			ApplyAnimation(animationName);
			EnableAnimation(true);
			StartAnim();
		}
	}

	public void StopAnimation(string animationName)
	{
		if (animationName.Equals(currentAnimationSetting.animName))
		{
			EnableAnimation(false);
		}
	}

	public void StopAnimations()
	{
		EnableAnimation(false);
	}

	public void Sample(string animationName, float position)
	{
		ApplyAnimation(animationName);
		SetupSprite();
		ULSpriteAnimModelInterface uLSpriteAnimModelInterface = animationModel;
		elapsed = position * ((float)uLSpriteAnimModelInterface.CellCount(animationName) * 1f / (float)uLSpriteAnimModelInterface.FramesPerSecond(animationName));
		UpdateSprite();
	}

	public float NormalizedTimePerFrame(string animationName)
	{
		ULSpriteAnimModelInterface uLSpriteAnimModelInterface = animationModel;
		float num = uLSpriteAnimModelInterface.FramesPerSecond(animationName);
		float num2 = num / ((Application.targetFrameRate >= 0) ? ((float)Application.targetFrameRate) : 60f);
		return 1f / (float)uLSpriteAnimModelInterface.CellCount(animationName) * num2;
	}
}
