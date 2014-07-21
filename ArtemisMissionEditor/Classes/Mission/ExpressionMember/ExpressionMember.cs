﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;

namespace ArtemisMissionEditor
{
	using EMVD = ExpressionMemberValueDescription;
	using EMVB = ExpressionMemberValueBehaviorInXml;
	using EMVT = ExpressionMemberValueType;
	using EMVE = ExpressionMemberValueEditor;
	
	/// <summary>
	/// Represents a single member in an expression, like (variable) in "If (variable) (comparator) (value)"
	/// </summary>
	public class ExpressionMember
	{
		/// <summary>
		/// Text label of the member, not sure how it will be used yet
		/// </summary>
		public string Text;

		protected ExpressionMemberValueDescription _valueDescription;
		/// <summary>
		/// Description of the value of this member - its type, boundaries, etc
		/// </summary>
		public ExpressionMemberValueDescription ValueDescription
		{
			get { return _valueDescription; }
			set { _valueDescription = value; OnValueDescriptionChange(); }
		}
		protected virtual void OnValueDescriptionChange() { }

		/// <summary>
		/// Must this member be specified a nonempty value
		/// </summary>
		public bool Mandatory;

		/// <summary>
		/// Name of the variable in xml, like: name, centerX, comparator etc
		/// </summary>
		public string Name;

		/// <summary>
		/// Behave as check or not
		/// </summary>
		public virtual bool IsCheck { get { return false; } }

		/// <summary>
		/// Require line break
		/// </summary>
		public virtual bool RequiresLinebreak { get { return false; } }

		/// <summary>
		/// Expressions nested within a Check
		/// </summary>
		public Dictionary<string, List<ExpressionMember>> PossibleExpressions;

		/// <summary>
		/// Decide which state should the check be in, and store the result
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public virtual string Decide(ExpressionMemberContainer container) { throw new Exception("FAIL! Calling Decide on a noncheck member"); }

		/// <summary>
		/// Get value of current member (internal value, as is in XML)
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public virtual string GetValue(ExpressionMemberContainer container)
		{
			return IsCheck ? container.CheckValue : container.GetAttribute();
		}

		/// <summary>
		/// Get value of current member to display in the label (=exactly what will appear in GUI)
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public virtual string GetValueDisplay(ExpressionMemberContainer container) { return ValueDescription.GetValueDisplay(container); }

		/// <summary>
		/// Get value of current member to display in the text string representing the statement
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public virtual string GetValueText(ExpressionMemberContainer container) { return GetValueDisplay(container); }

		/// <summary>
		/// Set value of the current member, including all checks (like, sets to null if this is a "store only if filled" value etc...)
		/// </summary>
		/// <param name="container"></param>
		/// <param name="value"></param>
		protected virtual void SetValueInternal(ExpressionMemberContainer container, string value) { container.SetAttribute(value); }

		public void SetValue(ExpressionMemberContainer container, string value) { SetValueInternal(container, value); container.Statement.Update(); }

		/// <summary>
		/// Occurs when control assigned to this member is clicked
		/// </summary>
		/// <param name="container"></param>
		public void OnClick(ExpressionMemberContainer container, NormalLabel l, Point curPos, EditorActivationMode mode) { ValueDescription.OnClick(container, l, curPos, mode); }

		/// <summary>
		/// Transform display value to xml
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public string ValueToXml(string value) { return ValueDescription.ValueToXml(value); }

		/// <summary>
		/// Transform XML value to display
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public string ValueToDisplay(string value) { return ValueDescription.ValueToDisplay(value); }

		/// <summary>
		/// Creates a fully functional expression member
		/// </summary>
		/// <param name="text"></param>
		/// <param name="valueDescription"></param>
		/// <param name="nameXml"></param>
		/// <param name="nameEditor">If not specified or specified null, the value of nameXml is used</param>
		public ExpressionMember(string text, ExpressionMemberValueDescription valueDescription, string nameXml = "", bool mandatory = false)
		{
			Text = text;
			ValueDescription = valueDescription;
			Name = nameXml;
			Mandatory = mandatory;
            PossibleExpressions = new Dictionary<string, List<ExpressionMember>>();
		}

		/// <summary>
		/// Creates a text-only expression member
		/// </summary>
		/// <param name="text"></param>
		public ExpressionMember(string text) : this(text, EMVD.GetItem("<label>")) { }

		public ExpressionMember() : this("", EMVD.GetItem("<blank>")) { }
	}

}