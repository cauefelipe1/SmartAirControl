using MediatR;
using SmartAirControl.Models.User;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAirControl.API.Features.User
{
    public class UserMediator
    {
        #region Get_User
        public class GetUserRequest : IRequest<UserModel>
        {
            public string Username { get; }

            public GetUserRequest(string username)
            {
                Username = username;
            }
        }

        public class GetUserHandler : IRequestHandler<GetUserRequest, UserModel>
        {
            private readonly IUserRepository _repo;

            public GetUserHandler(IUserRepository repository)
            {
                _repo = repository;
            }

            public Task<UserModel> Handle(GetUserRequest request, CancellationToken cancellationToken) => Task.Run(() =>
            {
                UserModel result = null;

                var dto = _repo.GetUser(request.Username);

                if (dto is not null)
                    result = BuildModel(dto);

                return result;
            });

            private UserModel BuildModel(UserDTO dto)
            {
                var model = new UserModel
                {
                    UserId = dto.UserId,
                    Username = dto.Username,
                    Type = (UserType)dto.UserType,
                    InsertTimestamp = dto.InsertTS
                };

                return model;
            }
        }
        #endregion Get_User

        #region Register_User
        public class RegisterUserRequest : IRequest<int>
        {
            public UserModel Model { get; }

            public RegisterUserRequest(UserModel model)
            {
                Model = model;
            }
        }

        public class RegisterUserHandler : IRequestHandler<RegisterUserRequest, int>
        {
            private readonly IUserRepository _repo;

            public RegisterUserHandler(IUserRepository repository)
            {
                _repo = repository;
            }

            public Task<int> Handle(RegisterUserRequest request, CancellationToken cancellationToken) => Task.Run(() =>
            {
                ValidateModel(request.Model);

                var dto = BuildDTO(request.Model);

                int userId = _repo.SaveUser(dto);

                return userId;
            });

            private UserDTO BuildDTO(UserModel model)
            {
                var dto = new UserDTO
                {
                    Username = model.Username,
                    Password = model.Password,
                    UserType = (int)model.Type,
                    InsertTS = DateTime.UtcNow
                };

                return dto;
            }

            public void ValidateModel(UserModel model)
            {
                if (model is null)
                    throw new Exception("Invalid model instance.");

                if (string.IsNullOrEmpty(model.Password))
                    throw new ArgumentNullException(nameof(model.Password), "A password must be informed.");
            }
        }
        #endregion Register_User
    }
}
