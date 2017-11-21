using Prism.Navigation;
using System.Collections.Generic;
using System.Linq;

namespace Blog.Demo.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private List<Models.Blog> _items;

        private List<Models.Blog> Items
        {
            get => _items;
            set
            {
                _items = value;
                RaisePropertyChanged(nameof(Items));
            }
        }

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Main Page";

            using (var db = new Data.BlogContext())
            {
                db.Database.EnsureCreated();

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

                db.AddRange(entries);
                db.SaveChanges();

                Items = db.Blogs.ToList();
            }
        }
    }
}
