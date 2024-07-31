namespace BusinessLayer.Middlewares;

public class BlogExceptionMessages
{
    public const string NotFound = "SubCategory not found";
    public const string Conflict = "Blog Already exist";
    public const string OrderNumberConflict = "Order number already exists";
}

public class CategoryExceptionMessages
{
    public const string NotFound = "Category not found";
    public const string Conflict = "Category Already exist";
    public const string OrderNumberConflict = "Order number already exists";
}

public class MenuItemExceptionMessages
{
    public const string NotFound = "Menu item not found.";
    public const string OrderNumberConflict = "Order number already exists";
}

public class SliderExceptionMessages
{
    public const string NotFound = "Slider not found";
    public const string Conflict = "Slider Already exist";
    public const string OrderNumberConflict = "Order number already exists";
}

public class SubCategoryExceptionMessages
{
    public const string NotFound = "SubCategory not found";
    public const string Conflict = "SubCategory Already exist";
    public const string OrderNumberConflict = "Order number already exists";
}

public class ContactFormExceptionMessages
{
    public const string NotFound = "Contact form not found";
}

public class UserExceptionMessage
{
    public const string NotFound = "User not found";
}

public class AuthenticationExceptionMessage
{
    public const string UserNotFound = "User not found";
    public const string WrongPassword = "Wrong Password";
    public const string EmailAlreadyExist = "Email already exist";
    public const string InvalidToken = "Invalid Token";
}

public class FeaturedCategoryExceptionMessages
{
    public const string CategoryConflict = "The same category cannot be highlighted twice.";
    public const string OrderNumberConflict = "Two featured category cannot have the same order number.";
}

public class FooterItemExceptionMessages
{
    public const string NotFound = "Footer item not found";
    public const string Conflict = "Footer Item Already exist";
    public const string OrderNumberConflict = "Order number already exists";
}

public class BrandExceptionMessages
{
    public const string NotFound = "Brand not found";
    public const string Conflict = "Brand Already exist";
}


public class FAQExceptionMessages
{
    public const string OrderNumberConflict = "Order number already exists";
    public const string NotFound = "FAQ not found";
    public const string LabelConflict = "Label already exists";
}

public class ImageExceptionMessages
{
    public const string NotFound = "Image not found";
    public const string InvalidImage = "Invalid image";
    public const string InvalidPath = "Invalid path";
    public const string InvalidName = "Invalid file name";
    
}

public class FeatureExceptionMessages
{
    public const string NotFound = "Feature not found";
}