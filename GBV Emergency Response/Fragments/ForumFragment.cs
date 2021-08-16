using System;
using System.Collections.Generic;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Com.Github.Library.Bubbleview;
using GBV_Emergency_Response.Models;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using Java.Util;

namespace GBV_Emergency_Response.Fragments
{
    public class ForumFragment : HelpFragment
    {
        private string SenderName;
        private Context context;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        private FloatingActionButton FabSend;
        private RecyclerView recycler;
        private TextInputEditText InputMessage;
        private readonly List<ForumMessage> items = new List<ForumMessage>();
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.forum_fragment, container, false);
            context = view.Context;
            ConnectViews(view);
            return view;
        }
        private void SendMsg()
        {
            HashMap msg = new HashMap();
            msg.Put("Message", InputMessage.Text);
            msg.Put("SenderName", SenderName);
            msg.Put("DateTime", DateTime.Now.ToString("dd MMM yyyy HH:mm tt"));
            msg.Put("SenderId", Firebase.Auth.FirebaseAuth.Instance.Uid);

            InputMessage.Text = string.Empty;
        }
        private void ConnectViews(View view)
        {
            FabSend = view.FindViewById<FloatingActionButton>(Resource.Id.FabSendMessage);
            InputMessage = view.FindViewById<TextInputEditText>(Resource.Id.InputReplyMessage);
            recycler = view.FindViewById<RecyclerView>(Resource.Id.RecyclerChatForum);
            FabSend.Click += FabSend_Click;

            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(context)
            {
                StackFromEnd = true
            };



            recycler.SetLayoutManager(linearLayoutManager);
            ChatAdapter adapter = new ChatAdapter(items, Firebase.Auth.FirebaseAuth.Instance.CurrentUser.Uid);
            recycler.SetAdapter(adapter);

            recycler.SmoothScrollToPosition(adapter.ItemCount);
        }


        private void FabSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InputMessage.Text.Trim()))
            {
                SendMsg();
            }
        }

    }
    class ChatAdapter : RecyclerView.Adapter
    {
        private readonly List<ForumMessage> items = new List<ForumMessage>();
        string KeyId;
        
        public ChatAdapter(List<ForumMessage> data, string key)
        {
            items = data;
            KeyId = key;
        }

        public override int ItemCount
        {
            get
            {
                return items.Count;
            }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is ChatsView)
            {
                ChatsView chatsView = holder as ChatsView;
                //chatsView.TxtTimeDate.Text = items[position].DateTime;
                chatsView.TxtMessage.Text = items[position].Msg;
                if(items[position].Date_Time.Date==DateTime.Now.Date)
                {
                    chatsView.TxtName.Text = $"{items[position].SenderName} 📅 today {items[position].Date_Time.ToString("HH:mm tt")}" ;
                }
                else
                {
                    chatsView.TxtName.Text = $"{items[position].SenderName} 📅({items[position].Date_Time.ToString("ddd, dd/MMM/yyyy HH:mm tt")})";
                }
            }
            else
            {
                SenderChats senderView = holder as SenderChats;
                senderView.SenderTxtMessage.Text = items[position].Msg;
                //senderView.SenderTxtTimeDate.Text = items[position].Date_Time.;
                if (items[position].Date_Time.Date == DateTime.Now.Date)
                {
                    senderView.SenderTxtTimeDate.Text = $"📅 today: {items[position].Date_Time.ToString("HH:mm tt")}";
                }
                else
                {
                    senderView.SenderTxtTimeDate.Text = $"📅{items[position].Date_Time.ToString("ddd, dd/MMM/yyyy HH:mm tt")}";
                }
            }

        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == Resource.Layout.sender_message_row)
            {
                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.sender_message_row, parent, false);
                BubbleTextView TxtMessage = row.FindViewById<BubbleTextView>(Resource.Id.TxtMessage);
                //TextView TxtDt = row.FindViewById<TextView>(Resource.Id.TxtMsgTime);
                TextView TxtName = row.FindViewById<TextView>(Resource.Id.TxtMsgSenderName);
                ChatsView view = new ChatsView(row)
                {
                    TxtMessage = TxtMessage,
                    //TxtTimeDate = TxtDt,
                    TxtName = TxtName

                };
                return view;
            }
            else
            {
                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.receiver_message_row, parent, false);
                BubbleTextView TxtMessage = row.FindViewById<BubbleTextView>(Resource.Id.SenderTxtMessage);
                TextView TxtDt = row.FindViewById<TextView>(Resource.Id.SenderTxtMsgTime);
                SenderChats view = new SenderChats(row)
                {
                    SenderTxtMessage = TxtMessage,
                    SenderTxtTimeDate = TxtDt,

                };
                return view;
            }

        }
        public override int GetItemViewType(int position)
        {
            if (items[position].UserId == KeyId)
            {
                return Resource.Layout.receiver_message_row;
            }
            else
            {
                return Resource.Layout.sender_message_row;
            }

        }
        public class ChatsView : RecyclerView.ViewHolder
        {
            public View Myview { get; set; }
            public BubbleTextView TxtMessage { get; set; }
            public TextView TxtName { get; set; }
            //public TextView TxtTimeDate { get; set; }
            public ChatsView(View itemView) : base(itemView)
            {
                Myview = itemView;
            }
        }
        public class SenderChats : RecyclerView.ViewHolder
        {
            public View Myview { get; set; }
            public BubbleTextView SenderTxtMessage { get; set; }
            public TextView SenderTxtTimeDate { get; set; }
            public SenderChats(View itemView) : base(itemView)
            {
                Myview = itemView;
            }
        }
    }
}