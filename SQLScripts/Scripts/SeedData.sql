SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([Id], [Name]) VALUES (1, N'User 1')
SET IDENTITY_INSERT [dbo].[User] OFF
GO
SET IDENTITY_INSERT [dbo].[UserPurchase] ON 

INSERT [dbo].[UserPurchase] ([Id], [UserId], [CurrencyIsoCode], [Amount], [PurchaseDate]) VALUES (1, 1, N'USD', CAST(50.000 AS Decimal(15, 3)), CAST(N'2023-09-01T10:00:00.000' AS DateTime))
INSERT [dbo].[UserPurchase] ([Id], [UserId], [CurrencyIsoCode], [Amount], [PurchaseDate]) VALUES (57, 1, N'BRL', CAST(50.000 AS Decimal(15, 3)), CAST(N'2023-09-03T00:00:00.000' AS DateTime))
INSERT [dbo].[UserPurchase] ([Id], [UserId], [CurrencyIsoCode], [Amount], [PurchaseDate]) VALUES (58, 1, N'USD', CAST(1.368 AS Decimal(15, 3)), CAST(N'2023-09-04T03:06:08.940' AS DateTime))
INSERT [dbo].[UserPurchase] ([Id], [UserId], [CurrencyIsoCode], [Amount], [PurchaseDate]) VALUES (59, 1, N'BRL', CAST(5.472 AS Decimal(15, 3)), CAST(N'2023-09-04T03:06:11.457' AS DateTime))
SET IDENTITY_INSERT [dbo].[UserPurchase] OFF
GO
ALTER TABLE [dbo].[UserPurchase]  WITH CHECK ADD  CONSTRAINT [FK_UserPurchase_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserPurchase] CHECK CONSTRAINT [FK_UserPurchase_UserId]
GO