using System.Collections;
using UnityEngine;

public class ULRenderTextureBatch
{
	private ArrayList batchList;

	private ULRenderTextureCameraRig renderTextureCameraRig;

	public ULRenderTextureCameraRig CameraRig
	{
		get
		{
			return renderTextureCameraRig;
		}
	}

	public ArrayList BatchList
	{
		get
		{
			return batchList;
		}
	}

	public ULRenderTextureBatch(int workingLayer)
	{
		batchList = new ArrayList();
		renderTextureCameraRig = new ULRenderTextureCameraRig(workingLayer);
	}

	public ULRenderTextureBatchEntry AddEntry(GameObject subject, int squareSize, string shaderIdentifier, ULRenderTextureCameraRig.RelativeCamDelegate camDelegate)
	{
		ULRenderTextureBatchEntry uLRenderTextureBatchEntry = new ULRenderTextureBatchEntry();
		uLRenderTextureBatchEntry.subject = subject;
		uLRenderTextureBatchEntry.target = new ULRenderTexture(squareSize, subject.name + "_material", shaderIdentifier);
		uLRenderTextureBatchEntry.camDelegate = camDelegate;
		batchList.Add(uLRenderTextureBatchEntry);
		return uLRenderTextureBatchEntry;
	}

	public ULRenderTextureBatchEntry AddEntry(GameObject subject, ULRenderTexture target, ULRenderTextureCameraRig.RelativeCamDelegate camDelegate)
	{
		ULRenderTextureBatchEntry uLRenderTextureBatchEntry = new ULRenderTextureBatchEntry();
		uLRenderTextureBatchEntry.subject = subject;
		uLRenderTextureBatchEntry.target = target;
		uLRenderTextureBatchEntry.camDelegate = camDelegate;
		batchList.Add(uLRenderTextureBatchEntry);
		return uLRenderTextureBatchEntry;
	}

	public void BatchUpdate(bool useCamDelegate)
	{
		foreach (ULRenderTextureBatchEntry batch in batchList)
		{
			renderTextureCameraRig.RenderSubjectToTexture(batch.subject, batch.target, (!useCamDelegate) ? null : batch.camDelegate);
		}
	}
}
