3/15：
当前正在做的功能：
	添加好友功能
好友请求流程：
	1）客户端发送 AddFriendRequest 后，服务器将该请求存入 FriendRequests 表，状态为“待处理”。
	2）若接收者在线，服务器立即推送一个 AddFriendRequest 通知包给接收者；若不在线，则等待其上线后拉取。
	3）接收方客户端收到请求通知后，应在主界面显示待处理请求列表（可新增一个“好友请求”面板），用户可选择接受或拒绝。
	4）接受时客户端发送 AcceptFriendRequest 包，服务器更新请求状态为“已接受”，并创建双向好友关系，然后通知双方更新好友列表。
	5）拒绝时发送 RejectFriendRequest 包，服务器更新状态为“已拒绝”。
        离线请求处理：
	6）在用户登录成功后，主动请求离线消息和好友请求，将离线期间收到的好友请求添加到待处理列表中。
	离线消息拉取
	在登录成功后添加：
	1）在 login 窗体登录成功、显示主窗体之前，调用 client.GetOfflineMessages()（需新增方法）获取未读消息和好友请求。
	2）服务器返回的离线消息列表，客户端应将其加入对应好友的聊天记录缓存，并更新好友列表项的未读计数。
	3）好友请求列表应显示在界面上（例如新增一个“好友请求”按钮，点击展开请求列表）。
	4）只需在 NetworkClient 中增加对应的请求方法和消息类型，并在服务器端添加相应的处理逻辑即可逐步完善。

分步骤处理：
	1）客户端发送 AddFriendRequest 后，服务器将该请求存入 FriendRequests 表，状态为“待处理”。
		UI界面调用AddFriend发送AddFriendRequest，注意：public bool AddFriend(string fromUserId, string toUserId)返回的是执行加好友操作是否成功，与成功添加好友没有关系
	2）若接收者在线，服务器立即推送一个 AddFriendRequest 通知包给接收者；若不在线，则等待其上线后拉取。
		UI界面订阅MessageReceived接受服务器发来的AddFriendRequest通知包，类型为AddFriendRequest，将其显示在通知界面
	3）接收方客户端收到请求通知后，应在主界面显示待处理请求列表（可新增一个“好友请求”面板），用户可选择接受或拒绝。
	4）接受时客户端发送 AcceptFriendRequest 包，服务器更新请求状态为“已接受”，并创建双向好友关系，然后通知双方更新好友列表。
		UI界面调用AcceptFriendRequest发送 AcceptFriendRequest 包
	5）拒绝时发送 RejectFriendRequest 包，服务器更新状态为“已拒绝”。
		UI界面调用RejectFriendRequest发送 RejectFriendRequest 包
	6）在用户登录成功后，主动请求离线消息和好友请求，将离线期间收到的好友请求添加到待处理列表中。
		在用户登录成功、主界面加载前（或加载后），主动调用一个方法（ GetOfflineNotifications）发送 GetNotificationListRequest 包，请求离线期间未处理的好友请求（及离线消息）。收到响应后，解析出待处理请求列表(List<string> friendRequest和List<Message> messages)，并将其添加到UI的通知列表中。