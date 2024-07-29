namespace BusinessLayer.Middlewares;

public class BlogExceptionMessages
{
    public const string NotFound = "SubCategory not found";
    public const string Conflict = "Already exist";
    public const string OrderNumberConflict = "Order number already exists";
}

public class CategoryExceptionMessages
{
    public const string NotFound = "Category not found";
    public const string Conflict = "Already exist";
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
    public const string Conflict = "Already exist";
    public const string OrderNumberConflict = "Order number already exists";
}

public class SubCategoryExceptionMessages
{
    public const string NotFound = "SubCategory not found";
    public const string Conflict = "Already exist";
    public const string OrderNumberConflict = "Order number already exists";
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