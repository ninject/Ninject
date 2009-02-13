using System;
using System.ComponentModel;

namespace Ninject.Syntax
{
	/// <summary>
	/// A hack to hide methods defined on <see cref="System.Object"/> for IntelliSense
	/// on fluent interfaces. Credit to Daniel Cazzulino.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IFluentSyntax
	{
		/// <inheritdoc/>
		[EditorBrowsable(EditorBrowsableState.Never)] Type GetType();

		/// <inheritdoc/>
		[EditorBrowsable(EditorBrowsableState.Never)] int GetHashCode();

		/// <inheritdoc/>
		[EditorBrowsable(EditorBrowsableState.Never)] string ToString();

		/// <inheritdoc/>
		[EditorBrowsable(EditorBrowsableState.Never)] bool Equals(object other);
	}
}