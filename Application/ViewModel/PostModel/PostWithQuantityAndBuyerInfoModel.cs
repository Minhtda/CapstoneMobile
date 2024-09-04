using Application.ViewModel.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.PostModel
{
    public class PostWithQuantityAndBuyerInfoModel
    {
        public List<PostViewModel> listPost {  get; set; }
        public UserDetailViewModel buyerInfo {  get; set; }
    }
}
