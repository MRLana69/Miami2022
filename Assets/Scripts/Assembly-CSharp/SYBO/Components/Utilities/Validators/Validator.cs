namespace SYBO.Components.Utilities.Validators
{
	public abstract class Validator<T>
	{
		public bool IsEnabled { get; set; }

		public Validator()
		{
			IsEnabled = true;
		}

		public abstract bool IsValid(T value);

		public abstract T Validate(T value);
	}
}
