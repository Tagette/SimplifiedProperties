/*
 * Author: Tristan Chambers
 * Date: Thursday, November 7, 2013
 * Email: Tristan.Chambers@hotmail.com
 * Website: Tristan.Heroic-Intentions.net
 */
using System;

namespace SMPL.Props {
	/// <summary>
	/// Property exception.
	/// </summary>
	public class PropertyException : Exception {

		#region Properties

		/// <summary>
		/// Gets or sets the entry.
		/// </summary>
		/// <value>The entry.</value>
		public PropertyEntry Entry { get; private set; }

		#endregion // Properties

		/// <summary>
		/// Initializes a new instance of the <see cref="SimplifiedProperties.PropertyException"/> class.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="entry">Entry.</param>
		public PropertyException (string @message, PropertyEntry @entry)
			: base (@message) {
			Entry = @entry;
		}
	}
}

