using Application.InterfaceService;
using Application.IService;
using AutoMapper;
using Domain.Entities;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class BackGroundService : IBackGroundService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ICurrentTime _currentTime;
        private readonly IPostService _postService;
        public BackGroundService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentTime currentTime, IPostService postService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentTime = currentTime;
            _postService = postService;
        }
        public async Task<bool> ExtendSubscription()
        {
            var isExtended = false;
            Wallet wallet = new Wallet();
            var listUser = await _unitOfWork.UserRepository.GetAllMember();
            foreach (var user in listUser)
            {
                try
                {
                    wallet = await _unitOfWork.WalletRepository.GetUserWalletByUserId(user.Id);
                }
                catch (Exception ex)
                {
                    Wallet newWallet = new Wallet()
                    {
                        OwnerId = user.Id,
                        UserBalance = 0,
                    };
                    await _unitOfWork.WalletRepository.AddAsync(newWallet);
                    await _unitOfWork.SaveChangeAsync();
                    var userWallet = await _unitOfWork.WalletRepository.FindWalletByUserId(user.Id);
                    var walletId = userWallet.Id;
                    user.WalletId = walletId;
                    _unitOfWork.UserRepository.Update(user);
                    await _unitOfWork.SaveChangeAsync();
                    isExtended = false;
                    continue;
                }
                var subscriptionHistoriesViewModel = await _unitOfWork.SubscriptionHistoryRepository.GetCurrentUserAvailableSubscripion(user.Id);
                foreach (var subscriptionHistoryViewModel in subscriptionHistoriesViewModel)
                {
                    var subscription = await _unitOfWork.SubcriptionRepository.GetByIdAsync(subscriptionHistoryViewModel.SubscriptionId);
                    var subscriptionHistory = await _unitOfWork.SubscriptionHistoryRepository.GetByIdAsync(subscriptionHistoryViewModel.Id);
                    if (subscriptionHistory.IsExtend == false)
                    {
                        if (subscriptionHistory.EndDate <= _currentTime.GetCurrentTime())
                        {
                            subscriptionHistory.Status = false;
                            _unitOfWork.SubscriptionHistoryRepository.Update(subscriptionHistory);
                            await _unitOfWork.SaveChangeAsync();
                        }
                    }
                    else
                    {
                        if (subscriptionHistory.EndDate >= _currentTime.GetCurrentTime())
                        {
                            isExtended = false;
                        }
                        else
                        {
                            var wallletTransaction = await _unitOfWork.WalletTransactionRepository.GetAllTransactionByUserId(user.Id);
                            float pendingTransaction = wallletTransaction?.Where(item => item.Action == "Purchase pending").Sum(item => item.Amount) ?? 0;
                            float cancleTransaction = wallletTransaction?.Where(item => item.Action == "Cancelled Pending").Sum(item => item.Amount) ?? 0;
                            float deniedTransaction = wallletTransaction?.Where(item => item.Action == "Purchase denied").Sum(item => item.Amount) ?? 0;
                            float completeTransaction = wallletTransaction?.Where(item => item.Action == "Purchase complete").Sum(_ => _.Amount) ?? 0;
                            if (wallet.UserBalance - pendingTransaction + cancleTransaction + deniedTransaction + completeTransaction < subscription.Price)
                            {
                                WalletTransaction walletTransaction = new WalletTransaction()
                                {
                                    TransactionType = "Extend subscription failed,user balance is not enough",
                                    Amount = 0,
                                    WalletId = wallet.Id
                                };
                                subscriptionHistory.Status = false;
                                _unitOfWork.SubscriptionHistoryRepository.Update(subscriptionHistory);
                                _unitOfWork.WalletTransactionRepository.AddAsync(walletTransaction);
                                await _unitOfWork.SaveChangeAsync();
                            }
                            else
                            {
                                wallet.UserBalance = wallet.UserBalance - subscription.Price;
                                WalletTransaction walletTransaction = new WalletTransaction()
                                {
                                    TransactionType = "Extend subscription successfully",
                                    WalletId = wallet.Id,
                                    Amount = subscription.Price
                                };
                                subscriptionHistory.Status = true;
                                subscriptionHistory.EndDate = subscriptionHistory.EndDate.AddDays(subscription.ExpiryDay);
                                _unitOfWork.SubscriptionHistoryRepository.Update(subscriptionHistory);
                                _unitOfWork.WalletTransactionRepository.AddAsync(walletTransaction);
                                _unitOfWork.WalletRepository.Update(wallet);
                                isExtended = await _unitOfWork.SaveChangeAsync() > 0;
                            }
                        }
                    }
                }
            }
            return isExtended;
        }

        public async Task<bool> ExtendSubscriptionByUserId(Guid userId)
        {
            var isExtended = false;
            var wallet = await _unitOfWork.WalletRepository.GetUserWalletByUserId(userId);
            var subscriptionHistoriesViewModel = await _unitOfWork.SubscriptionHistoryRepository.GetCurrentUserAvailableSubscripion(userId);
            foreach (var subscriptionHistoryViewModel in subscriptionHistoriesViewModel)
            {
                var subscription = await _unitOfWork.SubcriptionRepository.GetByIdAsync(subscriptionHistoryViewModel.SubscriptionId);
                var subscriptionHistory = await _unitOfWork.SubscriptionHistoryRepository.GetByIdAsync(subscriptionHistoryViewModel.Id);
                if (subscriptionHistory.IsExtend == false)
                {
                    if (subscriptionHistory.EndDate <= _currentTime.GetCurrentTime())
                    {
                        subscriptionHistory.Status = false;
                        _unitOfWork.SubscriptionHistoryRepository.Update(subscriptionHistory);
                        await _unitOfWork.SaveChangeAsync();
                    }
                }
                else
                {
                    var wallletTransaction = await _unitOfWork.WalletTransactionRepository.GetAllTransactionByUserId(userId);
                    float pendingTransaction = wallletTransaction?.Where(item => item.Action == "Purchase pending").Sum(item => item.Amount) ?? 0;
                    float cancleTransaction = wallletTransaction?.Where(item => item.Action == "Cancelled Pending").Sum(item => item.Amount) ?? 0;
                    float deniedTransaction = wallletTransaction?.Where(item => item.Action == "Purchase denied").Sum(item => item.Amount) ?? 0;
                    float completeTransaction = wallletTransaction?.Where(item => item.Action == "Purchase complete").Sum(_ => _.Amount) ?? 0;
                    if (wallet.UserBalance - pendingTransaction + cancleTransaction + deniedTransaction + completeTransaction < subscription.Price)
                    {
                        WalletTransaction walletTransaction = new WalletTransaction()
                        {
                            TransactionType = "Extend subscription failed,user balance is not enough",
                            WalletId = wallet.Id
                        };
                        subscriptionHistory.Status = false;
                        _unitOfWork.SubscriptionHistoryRepository.Update(subscriptionHistory);
                        _unitOfWork.WalletTransactionRepository.AddAsync(walletTransaction);
                    }
                    else
                    {
                        wallet.UserBalance = wallet.UserBalance - subscription.Price;
                        WalletTransaction walletTransaction = new WalletTransaction()
                        {
                            TransactionType = "Extend subscription successfully",
                            WalletId = wallet.Id,
                            Amount = subscription.Price
                        };
                        subscriptionHistory.Status = true;
                        subscriptionHistory.EndDate = subscriptionHistory.EndDate.AddDays(subscription.ExpiryDay);
                        _unitOfWork.SubscriptionHistoryRepository.Update(subscriptionHistory);
                        _unitOfWork.WalletTransactionRepository.AddAsync(walletTransaction);
                        _unitOfWork.WalletRepository.Update(wallet);
                        isExtended = await _unitOfWork.SaveChangeAsync() > 0;
                        BackgroundJob.Schedule(() => (ExtendSubscriptionByUserId(userId)), TimeSpan.FromDays(subscription.ExpiryDay));
                    }
                }

            }
            if (!isExtended)
            {
                var checkDelete = await RemovePostWhenSubscriptionExpireByUserId(userId);
                return checkDelete;
            }
            return isExtended;
        }

        public async Task<bool> RemovePostWhenSubscriptionExpire()
        {
            bool isDeleted = false;
            var listUser = await _unitOfWork.UserRepository.GetAllMember();
            foreach(var user in listUser)
            {
                var listUserPurchaseSubscription=await _unitOfWork.SubscriptionHistoryRepository.GetUserPurchaseSubscription(user.Id);
               /* var listUserExpireSubscription = await _unitOfWork.SubscriptionHistoryRepository.GetUserExpireSubscription(user.Id);*/
                if (listUserPurchaseSubscription != null)
                {
                    if (listUserPurchaseSubscription.Count() > 0)
                    {
                        if (listUserPurchaseSubscription.Where(x => x.Status == "Expried").Count() == listUserPurchaseSubscription.Count())
                        {
                            var listPostCreatedByUser = await _unitOfWork.PostRepository.GetAllPostsByCreatedByIdAsync(user.Id);
                            if (listPostCreatedByUser != null)
                            {
                                foreach (var post in listPostCreatedByUser)
                                {
                                    if (post.Product.ConditionId != 3 )
                                    {
                                        _unitOfWork.PostRepository.SoftRemove(post);
                                        isDeleted = await _unitOfWork.SaveChangeAsync() > 0;
                                    }
                                }
                            }
                        }
                    }
                    
                } else
                {
                    var listPostCreatedByUser = await _unitOfWork.PostRepository.GetAllPostsByCreatedByIdAsync(user.Id);
                    if (listPostCreatedByUser != null)
                    {
                        foreach (var post in listPostCreatedByUser)
                        {
                            if (post.Product.ConditionId != 3)
                            {
                                _unitOfWork.PostRepository.SoftRemove(post);
                                isDeleted = await _unitOfWork.SaveChangeAsync() > 0;
                            }
                        }
                    }
                }
            }
            return isDeleted;
        }

        public async Task<bool> RemovePostWhenSubscriptionExpireByUserId(Guid userId)
        {
            var isDeleted = false;
            var listPostCreatedByUser = await _unitOfWork.PostRepository.GetAllPostsByCreatedByIdAsync(userId);
            if (listPostCreatedByUser != null)
            {
                foreach (var post in listPostCreatedByUser)
                {
                    if (post.Product.ConditionId != 3)
                    {
                        _unitOfWork.PostRepository.SoftRemove(post);
                        isDeleted = await _unitOfWork.SaveChangeAsync() > 0;
                    }
                }
            }
            return isDeleted;
        }
    }
}
