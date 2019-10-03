using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.Platforms.Android.Binding.Views;
using MvvmCross.ViewModels;
using MvvmCross.WeakSubscription;

namespace Timeinator.Mobile.Android
{
    [Preserve(AllMembers = true)]
    public class LinkerPleaseInclude
    {
        private readonly Dictionary<MethodInfo, string> mMethodInfoToUnitSuffix = new Dictionary<MethodInfo, string>
        {
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddYears), new[] { typeof(int) }), " years" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddMonths), new[] { typeof(int) }), " months" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddDays), new[] { typeof(double) }), " days" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddHours), new[] { typeof(double) }), " hours" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddMinutes), new[] { typeof(double) }), " minutes" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddSeconds), new[] { typeof(double) }), " seconds" }
        };

        private readonly Dictionary<MethodInfo, string> mSupportedMethods = new Dictionary<MethodInfo, string>
        {
            { typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(double) }), "abs" },
            { typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(float) }), "abs" },
            { typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(int) }), "abs" },
            { typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(long) }), "abs" },
            { typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(sbyte) }), "abs" },
            { typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(short) }), "abs" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(byte), typeof(byte) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(double), typeof(double) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(float), typeof(float) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(int), typeof(int) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(long), typeof(long) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(sbyte), typeof(sbyte) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(short), typeof(short) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(uint), typeof(uint) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(ushort), typeof(ushort) }), "max" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(byte), typeof(byte) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(double), typeof(double) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(float), typeof(float) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(int), typeof(int) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(long), typeof(long) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(sbyte), typeof(sbyte) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(short), typeof(short) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(uint), typeof(uint) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Min), new[] { typeof(ushort), typeof(ushort) }), "min" },
            { typeof(Math).GetMethod(nameof(Math.Round), new[] { typeof(double) }), "round" },
            { typeof(Math).GetMethod(nameof(Math.Round), new[] { typeof(double), typeof(int) }), "round" }
        };

        public void Include(Button button)
        {
            button.Click += (s, e) => button.Text = button.Text + "";
        }

        public void Include(CheckBox checkBox)
        {
            checkBox.CheckedChange += (sender, args) => checkBox.Checked = !checkBox.Checked;
        }

        public void Include(Switch @switch)
        {
            @switch.CheckedChange += (sender, args) => @switch.Checked = !@switch.Checked;
        }

        public void Include(FloatingActionButton button)
        {
            button = new FloatingActionButton(null);
            button.Click += (s, e) => button.TooltipText = "";
        }

        public void Include(View view)
        {
            view.Click += (s, e) => view.ContentDescription = view.ContentDescription + "";
        }

        public void Include(TextView text)
        {
            text.AfterTextChanged += (sender, args) => text.Text = "" + text.Text;
            text.Hint = "" + text.Hint;
        }

        public void Include(CheckedTextView text)
        {
            text.AfterTextChanged += (sender, args) => text.Text = "" + text.Text;
            text.Hint = "" + text.Hint;
        }

        public void Include(CompoundButton cb)
        {
            cb.CheckedChange += (sender, args) => cb.Checked = !cb.Checked;
        }

        public void Include(SeekBar sb)
        {
            sb.ProgressChanged += (sender, args) => sb.Progress = sb.Progress + 1;
        }

        public void Include(Activity act)
        {
            act.Title = act.Title + "";
        }

        public void Include(MvxLinearLayout mvxLinearLayout)
        {
            mvxLinearLayout = new MvxLinearLayout(null, null);
        }

        public void Include(MvxRecyclerView mvxLinearLayout)
        {
            mvxLinearLayout = new MvxRecyclerView(null, null);
            mvxLinearLayout.ChildViewAdded += (s, e) => mvxLinearLayout = null;
            mvxLinearLayout.ViewAttachedToWindow += (s, e) => mvxLinearLayout = null;

            mvxLinearLayout.OnChildAttachedToWindow(null);
        }

        public void Include(RecyclerView mvxLinearLayout)
        {
            mvxLinearLayout = new RecyclerView(null, null);
            mvxLinearLayout.ChildViewAdded += (s, e) => mvxLinearLayout = null;
            mvxLinearLayout.ViewAttachedToWindow += (s, e) => mvxLinearLayout = null;

            mvxLinearLayout.OnChildAttachedToWindow(null);
        }

        public void Include(MvvmCross.Droid.Support.V7.RecyclerView.ItemTemplates.MvxDefaultTemplateSelector injector)
        {
            injector = new MvvmCross.Droid.Support.V7.RecyclerView.ItemTemplates.MvxDefaultTemplateSelector();
        }

        public void Include(IMvxRecyclerViewHolder viewHolder)
        {
            void OnItemViewClick(object sender, EventArgs e) { }
            void OnItemViewLongClick(object sender, EventArgs e) { }

            viewHolder.Click -= OnItemViewClick;
            viewHolder.LongClick -= OnItemViewLongClick;
            viewHolder.Click += OnItemViewClick;
            viewHolder.LongClick += OnItemViewLongClick;
        }

        public void Include(RecyclerView.ViewHolder vh, MvxRecyclerView list)
        {
            vh.ItemView.Click += (sender, args) => { };
            vh.ItemView.LongClick += (sender, args) => { };
            list.ItemsSource = null;
            list.Click += (sender, args) => { };
        }

        public void Include(MvxWeakEventSubscription<RecyclerView, RecyclerView> @event)
        {
            @event.WeakSubscribe(null, null);
        }

        public void Include(INotifyCollectionChanged changed)
        {
            changed.CollectionChanged += (s, e) => {
                var test =
$"{e.Action}{e.NewItems}{e.NewStartingIndex}{e.OldItems}{e.OldStartingIndex}";
            };
        }

        public void Include(ICommand command)
        {
            command.CanExecuteChanged += (s, e) => { if (command.CanExecute(null)) command.Execute(null); };
        }

        public void Include(MvxPropertyInjector injector)
        {
            injector = new MvxPropertyInjector();
        }

        public void Include(System.ComponentModel.INotifyPropertyChanged changed)
        {
            changed.PropertyChanged += (sender, e) =>
            {
                var test = e.PropertyName;
            };
        }

        public void Include(RadioGroup radioGroup)
        {
            radioGroup.CheckedChange += (sender, args) => radioGroup.Check(args.CheckedId);
        }

        public void Include(RadioButton radioButton)
        {
            radioButton.CheckedChange += (sender, args) => radioButton.Checked = args.IsChecked;
        }

        public void Include(RatingBar ratingBar)
        {
            ratingBar.RatingBarChange += (sender, args) => ratingBar.Rating = 0 + ratingBar.Rating;
        }

        public void Include(MvxTaskBasedBindingContext context)
        {
            context.Dispose();
            var context2 = new MvxTaskBasedBindingContext();
            context2.Dispose();
        }

        public void Include(MvxNavigationService service, IMvxViewModelLoader loader)
        {
            service = new MvxNavigationService(null, loader);
        }

        public void Include(MvxListView listview)
        {
            listview.ItemsSource = new List<int>();
            var itemsSource = listview.ItemsSource;
        }

        public void Include(ConsoleColor color)
        {
            Console.Write("");
            Console.WriteLine("");
            color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }
    }
}