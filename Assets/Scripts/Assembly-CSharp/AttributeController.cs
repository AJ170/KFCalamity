using System.Collections.Generic;
using UnityEngine;

public class AttributeController : MonoBehaviour
{
	public List<AttributeEditorData> DefaultAttributes = new List<AttributeEditorData>();

	private List<AttributeBase> attributes = new List<AttributeBase>();

	private void Awake()
	{
		SetupDefaultAttributes();
	}

	private void Update()
	{
		RecalculateModifierAttributes();
	}

	private void SetupDefaultAttributes()
	{
		foreach (AttributeEditorData defaultAttribute in DefaultAttributes)
		{
			if (!IsAttributeRegistered(defaultAttribute.Name))
			{
				if (defaultAttribute.Type == AttributeType.Flag)
				{
					RegisterAttribute(new AttributeFlag(defaultAttribute.Name, defaultAttribute.Flag));
				}
				else if (defaultAttribute.Type == AttributeType.Modifier)
				{
					RegisterAttribute(new AttributeModifier(defaultAttribute.Name, defaultAttribute.Fixed, defaultAttribute.Multiplier));
				}
			}
			else
			{
				Debug.LogError(string.Format("AttributeController:SetupDefaultAttributes() -> {0} already registered. Maybe duplicated?", defaultAttribute.Name.ToString()));
			}
		}
	}

	public static T CreateAttribute<T>(AttributeName name, GameObject go) where T : AttributeBase
	{
		AttributeController attributeController = ControllerByGameObject(go);
		T val = (T)null;
		bool flag = true;
		if (attributeController != null && attributeController.IsAttributeRegistered(name))
		{
			val = attributeController.AttributeByName(name) as T;
			flag = false;
		}
		if (flag)
		{
			if (typeof(T) == typeof(AttributeFlag))
			{
				val = new AttributeFlag(name) as T;
			}
			else if (typeof(T) == typeof(AttributeModifier))
			{
				val = new AttributeModifier(name) as T;
			}
			else
			{
				Debug.LogError(string.Format("AttributeController:CreateAttribute() AttributeBase not supported (trace: {0})", go.name));
			}
			if (attributeController != null)
			{
				attributeController.RegisterAttribute(val);
			}
		}
		return (!(go == null)) ? val : ((T)null);
	}

	public void RegisterAttribute(AttributeBase attribute)
	{
		if (attribute != null && !IsAttributeRegistered(attribute.Name))
		{
			if (attribute.Name != 0)
			{
				attributes.Add(attribute);
			}
			else
			{
				Debug.LogError(string.Format("AttributeController:RegisterAttribute() -> Tried to register '{0}' for {1}", attribute.Name.ToString(), base.transform.root.name));
			}
		}
	}

	private void RecalculateModifierAttributes()
	{
		foreach (AttributeBase attribute in attributes)
		{
			if (attribute.Type == AttributeType.Modifier)
			{
				((AttributeModifier)attribute).Recalculate();
			}
		}
	}

	public AttributeBase AttributeByName(AttributeName name)
	{
		foreach (AttributeBase attribute in attributes)
		{
			if (name == attribute.Name)
			{
				return attribute;
			}
		}
		return null;
	}

	public bool IsAttributeRegistered(AttributeName name)
	{
		foreach (AttributeBase attribute in attributes)
		{
			if (name == attribute.Name)
			{
				return true;
			}
		}
		return false;
	}

	public static AttributeController ControllerByGameObject(GameObject go)
	{
		if (go != null)
		{
			AttributeController componentInRoot = go.GetComponentInRoot<AttributeController>();
			if (componentInRoot != null)
			{
				return componentInRoot;
			}
		}
		else
		{
			Debug.LogError("AttributeController:ControllerByGameObject() Missing gameObject!");
		}
		return null;
	}
}
