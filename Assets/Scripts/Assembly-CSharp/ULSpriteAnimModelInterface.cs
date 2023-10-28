using System.Collections.Generic;

public interface ULSpriteAnimModelInterface
{
	string GetMaterialName(string animName);

	string GetResourceName(string animName);

	string GetTextureName(string animName);

	bool HasAnimation(string animName);

	float CellTop(string animName);

	float CellLeft(string animName);

	float CellWidth(string animName);

	float CellHeight(string animName);

	int CellStartColumn(string animName);

	int CellColumns(string animName);

	int CellCount(string animName);

	int FramesPerSecond(string animName);

	float TimingTotal(string animName);

	List<float> TimingList(string animName);

	bool Loop(string animName);

	bool FlipH(string animName);

	bool FlipV(string animName);
}
