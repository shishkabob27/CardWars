public class SpellScript : CardScript
{
	public virtual void Cast()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}
}
