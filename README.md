# Xamarin Forms with Entity Framework

A quick introduction to get started with XF 2.5.x and .Net Standards v2.0, by creating an application that reads and writes data to a SQLite database via Entity Framework Core 2.0.

To get started, we need to ensure that we have the latest version of Visual Studio 2017. At the time of writing this, .Net 4.7 is not readily supported in Visual Studio for Mac, but this does not mean you cannot use .Net Standard 2.0 in VS for Mac, you just need to target .Net Framework 4.6.2.

To ensure you have the latest updates of Visual Studio (Windows), go to Help - About and identify your version as 15.4.4.

VS 15.4.4 comes with a host of updates to tools such as Xamarin, better support for Android Orea and emulation and of course, and iPhone X emulator (if you have Enterprise edition). Similar updates have been made to VS for Mac 7.2.x, including a stable support for XCode 9.1 and the OSx agents.

The following screen grab shows the latest release of Visual Studio 2017:

![](https://raw.githubusercontent.com/JadedEric/xamarin-netstandard/master/images/01.png)

Next thing we need to make sure of, is that we have the Prism 7.x pre-release template pack installed. Take note, however, that if you update the template pack over the existing version 6.x pack, that your applications might not work. Do this in an isolated environment for testing only.

To grab the template pack for Prism, we utilize Visual Studio's extension installer by going through Tools - Extensions and Updates then in the left hand navigation items going through Online - Visual Studio Market then in the search bar, on the right hand side, looking for "Prism Template Pack":

![](https://raw.githubusercontent.com/JadedEric/xamarin-netstandard/master/images/02.png)

Notice that the version for the template pack is not 7.x, rather sitting at a version 2.0.7. The reason for the template pack is so that we can create a Prism-derived application from a template, rather than hand rolling the necessary dependencies into our applications manually. These templates also injects the necessary 3rd party assemblies required to compile and run Prism MVVM again your specific target platform, whether this is Android, iOS or Windows Phone.

Now that we have this installed, restart Visual Studio to apply the extension changes and we're ready to kick off.

Create a new project in Visual Studio and choose Prism as your application derived template. To create a new project, you can go to File - New - Project or from the Start Page, Create New Project:

![](https://raw.githubusercontent.com/JadedEric/xamarin-netstandard/master/images/03.png)

A few notes on the screen grab above:

1.) My target framework is .Net 4.7. The reason for this is so that I can utilize the latest updates to the C# version 7.1; I am not covering these updates in this blog, should you like to know more have a look at this blog
2.) I chose Prism under the C# language reference; the template I chose is a blank application for XF
3.) Give your application a name, store it somewhere and if you want to, add it to a Git repository

Next up is choosing your platform and specifying your dependency container:

![](https://raw.githubusercontent.com/JadedEric/xamarin-netstandard/master/images/04.png)

I chose Microsoft's Unity (not Unity the game engine) as I'm comfortable with Unity and it's interpretation of dependency control, and a single Android application to make my life slightly easier in developing and testing this application.

Once the application has been created, you should have a solution that looks something like this:

![](https://raw.githubusercontent.com/JadedEric/xamarin-netstandard/master/images/05.png)

Notice that the shared library called Blog.Demo, has a warning on the dependencies? This is because common service locater, which is used by Unity to inject interface services into the container stack is still a PCL library and Visual Studio is warning you that it might not work in your current configuration. This is not a problem and you can safely ignore this warning as your application will still be compiled. We will fix this issue by updating our dependencies.

Update your solution's dependencies to ensure that any new updates to the libraries have been brought down. It is a general rule of thumb to keep your packages up to date; do this by right-clicking on the solution, choosing Manage NuGet Packages:

![](https://raw.githubusercontent.com/JadedEric/xamarin-netstandard/master/images/06.png)

You should be presented with at least 7+ updates. Select all packages and update. Remember, rule of thumb is to keep them up to date.

Once the update has been completed, you're ready to start for the changes to take effect.

We need to add one more NuGet to the Droid project, in order for deployment to device / emulator to be successful. Xamarin introduces a new pointer definition which fails on Android 7+ environments, so right-click on the *.Droid project and click on Manage NuGet Packages. Under the Browse tab, search for the package called "System.Runtime.CompilerServices.Unsafe" and ensure you select version 4.3.0, not 4.4.0 which is the latest version. Android 7+ with mono touch cannot compile 4.4.0, and is a known issue, as of time of writing this block.

The application we'll be building is a simple sort-and-retrieve application, not the boring To-Do applications you find on the net, but something a bit more challenging, to show how .Net Standards shines above it's predecessor, Portable Class Library.

For purpose of this demo, you'll notice that I've split the application into various modular sections, called "projects". I do this, so that I have greater maintainability over my code. I can also write unit tests for my projects without a dependency on my main shared library, affording a better code coverage to my overall code maintainability. This blog does not cover unit testing, but the tests as well as code coverage report has been included in the source code, should you wish to have a look at those.

To start off with, we need a way of storing and retrieving information from a local store, in this case, a local SQLite database. Previously, you needed to do quite a set up to get SQLite going, by either hand rolling your SQLite statements to create the DB, the table and then inserting into it. You also have the option to user Azure's Mobile Apps SDK to create this for you, however, I'd like to show  what .Net Standards bring to the party by enabling our application to use Entity Framework 7 to handle the hard work for us. If you have dealt with EF before, and have done database migrations, this should be nothing new, for those who have not, it's actually really simple.

Create a new .Net Standard library in your project, and call it something in the line of "DataLayer", I called my project Blog.Data; delete the Class1.cs file as we don't need this at the moment.

Next, add a new NuGet package to the solution by right-clicking on the Dependencies folder, and choosing Manage NuGet Packages. What we're looking for here is Entity Framework Core, so switch to the Browse tap (if not already there) and type in "microsoft.entityframeworkcore.sqlite" in the search bar. Do not install the Entity Framework package on the landing page of the browse tab, as this is EF 6.2 for C# applications and is not Mono ready.

Once installed, we need to create a new database context from which we will connect to our local database; create a new *.cs file in the data project, and call it DemoContext.cs, your project tree should look something like this:

![](https://raw.githubusercontent.com/JadedEric/xamarin-netstandard/master/images/07.png)

In the BlogContext.cs file, remove the class entry and replace it with the following snippet. This simply tells us that the BlogContext class implements the DbContext abstract class, allowing us to interject our database specific implementation on top of Entity Framework's default implementation:

```cs
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class BlogContext: DbContext
    {

    }
}
```

Notice the "using" statement above? This is so that we can get access to Entity Framework's base class implementation.

Next thing we need to do is to add a default constructor, so go ahead and add that in now, and do not implement anything more than a blank base(), we don't need to pass it anything (hint at dependency injection here). After that, override the method called OnConfiguration(...) delete the base implementation and replace it with the implementation below. This basically just tells the entity framework migrations that SQLite will be used as the platform for data storage, and the file that it should be writing to. You'll notice that my filename is empty, this is on purpose as we'll get to this a bit later in the tutorial.

 We're now ready to set up our data model for this tutorial, so go ahead and add a new .Net Standard library to the solution and call it something like Models; I called mine "```Blog.Models```". I also added another .Net Standard library called ```Blog.Core``` which will be used to store core specific functionality like extension methods, structs, enums, etc.

In the models project, add a new CS file called Blog.cs.

In the core project, add a new CS file called Settings.cs

Go back to the Data project, and in the context file, add a new public property of type ```DbSet<Blog>```, calling it Blogs; the context cs file should look something like this:

```cs
public class BlogContext: DbContext
{
    public DbSet<Models.Blog> Blogs { get; set; }

    public BlogContext(): base()
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"FileName={""}");
    }
}
```

At this point you should get red squiggles at the bottom of the ```DbSet<Blog>```, that's because we're not referencing our Models project in our Data projects, so right-click on Dependencies and add a new Reference to the Models project:

![](https://raw.githubusercontent.com/JadedEric/xamarin-netstandard/master/images/08.png)

```Core.Settings``` should look something like this:

```cs
public static class Settings
{
    public static string DbPath = string.Empty;
}
```

Why am I separating my settings into an external project? Force of habit. I come from an background of modular development and everything has to be sustainable. Should I need to pull my settings into another project, I can easily just implement the assembly knowing that I don't have to duplicate code, and that unit testing on this has already been completed.

Update the ```OnConfiguration(...)``` to look like this now, remembering to add a reference in the Data project to the Core project:

```cs
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlite($"FileName={Core.Settings.DbPath}");
}
```

That's it for setting up our data context. All we now need to do is tell our main application to create the db if not created.

Head over to your main view's view model implementation, usually called MainPageViewModel in the default .Net Standards library created by the Prism template engine and ensure the constructor looks something like this:

```cs
public class MainPageViewModel : ViewModelBase
{
    private List<Models.Blog> items;
    public List<Models.Blog> Items
    {
        get
        {
            return items;
        }
        set
        {
            items = value;
            RaisePropertyChanged(nameof(Items));
        }
    }

    public MainPageViewModel(INavigationService navigationService)
        : base (navigationService)
    {
        Title = "Main Page";

        using (var db = new Data.BlogContext())
        {
            db.Database.EnsureCreated();

            var entries = new List<Models.Blog>
            {
                new Models.Blog
                {

                },
                new Models.Blog
                {

                },
                new Models.Blog
                {

                }
            };

            db.AddRange(entries);
            db.SaveChanges();

            Items = db.Blogs.ToList();
        }
    }
}
```

All this snippet does is to ensure that the database has been created, and create it if not. Then we construct a new collection of blog entries which we add to the db context for adding into the SQLite store. After we have done that, we need to save the changes down to the file, by calling the ```SaveChanges()``` method on the db context.

The last part of the code, simply assigns the list of items from the database to an Items collection we're going to apply to a ListView to show that we got data back from the database.

At this point the application won't really do much. Why? Because don't have any properties to bind to on the Blog model, so head over to the model and add three properties for us to bind to: Id, Name and Rating:

```cs
public class Blog
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Rating { get; set; }
}
```

Then change the implementation in the MainPageViewModel to something like this:

```cs
var entries = new List<Models.Blog>
{
    new Models.Blog
    {
        Id = 1,
        Name = "Blog entry 1",
        Rating = 5
    },
    new Models.Blog
    {
        Id = 2,
        Name = "Blog entry 2",
        Rating = 10
    },
    new Models.Blog
    {
        Id = 3,
        Name = "Blog entry 3",
        Rating = 5
    }
};
```

The last thing we need to do, is get the directory and name where we'll be storing the database back from Android, so head over to the App.xaml.cs file in the base .Net Standard library and change the ```OnInitialized()``` method to look like this:

```cs
protected override async void OnInitialized()
{
    InitializeComponent();

    Core.Settings.DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "blog.db"); // <-- add this line

    await NavigationService.NavigateAsync("NavigationPage/MainPage");
}
```

f you've followed this example, you should have .Net Standard library implemented into a Xamarin Forms application using Prism 7.x pre-release.

This is just one area where .Net Standards 2.0 has an advantage over PCL implementation, taking into consideration that the entire .Net Core library now became available to you to leverage off.

If you have any queries or questions, pop me a message on my GitHub or if you're a Tangeneer pop me a chat on Teams and we can discuss your queries and questions further.

In the next blog, I'll tackle Entity Framework Migrations.