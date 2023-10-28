using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NisComponent : MonoBehaviour
{
	private bool _showSkipPrompt;

	public bool ShowSkipPrompt
	{
		get
		{
			return _showSkipPrompt;
		}
	}

	[method: MethodImpl(32)]
	public event Action<NisComponent> onComplete;

	[method: MethodImpl(32)]
	public event Action<NisComponent, bool> onShowSkipPrompt;

	public NisComponent(bool showSkipPromptInitial = true)
	{
		_showSkipPrompt = showSkipPromptInitial;
	}

	protected virtual void SetShowSkipPrompt(bool showPrompt)
	{
		if (showPrompt != _showSkipPrompt)
		{
			_showSkipPrompt = showPrompt;
			if (this.onShowSkipPrompt != null)
			{
				this.onShowSkipPrompt(this, showPrompt);
			}
		}
	}

	protected virtual void SetComplete()
	{
		if (this.onComplete != null)
		{
			this.onComplete(this);
		}
	}
}
