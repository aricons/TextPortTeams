using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Helpers.CustomValidation
{
    public class RequiredIfAttribute : ValidationAttribute, IClientValidatable
    {
        private String PropertyName { get; set; }
        private Object DesiredValue { get; set; }
        private readonly RequiredAttribute _innerAttribute;

        public RequiredIfAttribute(String propertyName, Object desiredvalue)
        {
            PropertyName = propertyName;
            DesiredValue = desiredvalue;
            _innerAttribute = new RequiredAttribute();
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var dependentValue = context.ObjectInstance.GetType().GetProperty(PropertyName).GetValue(context.ObjectInstance, null);

            if (dependentValue != null && dependentValue.ToString() == DesiredValue.ToString())
            {
                if (!_innerAttribute.IsValid(value))
                {
                    return new ValidationResult(FormatErrorMessage(context.DisplayName), new[] { context.MemberName });
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessageString,
                ValidationType = "requiredif",
            };
            rule.ValidationParameters["dependentproperty"] = PropertyName;
            rule.ValidationParameters["desiredvalue"] = DesiredValue is bool ? DesiredValue.ToString().ToLower() : DesiredValue;

            yield return rule;
        }
    }
}

// Original
//public class RequiredIfAttribute : ValidationAttribute, IClientValidatable
//{
//    private String PropertyName { get; set; }
//    private Object DesiredValue { get; set; }
//    private readonly RequiredAttribute _innerAttribute;

//    public RequiredIfAttribute(String propertyName, Object desiredvalue)
//    {
//        PropertyName = propertyName;
//        DesiredValue = desiredvalue;
//        _innerAttribute = new RequiredAttribute();
//    }

//    protected override ValidationResult IsValid(object value, ValidationContext context)
//    {
//        var dependentValue = context.ObjectInstance.GetType().GetProperty(PropertyName).GetValue(context.ObjectInstance, null);

//        if (dependentValue.ToString() == DesiredValue.ToString())
//        {
//            if (!_innerAttribute.IsValid(value))
//            {
//                return new ValidationResult(FormatErrorMessage(context.DisplayName), new[] { context.MemberName });
//            }
//        }
//        return ValidationResult.Success;
//    }

//    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
//    {
//        var rule = new ModelClientValidationRule
//        {
//            ErrorMessage = ErrorMessageString,
//            ValidationType = "requiredif",
//        };
//        rule.ValidationParameters["dependentproperty"] = (context as ViewContext).ViewData.TemplateInfo.GetFullHtmlFieldId(PropertyName);
//        rule.ValidationParameters["desiredvalue"] = DesiredValue is bool ? DesiredValue.ToString().ToLower() : DesiredValue;

//        yield return rule;
//    }
//}
//}