using System.Threading.Tasks;
using Grpc.Core;
using GrpcServiceDemo.Server.Models;
using Microsoft.EntityFrameworkCore;
using UserService;

namespace GrpcServiceDemo.Server.Services
{
    // UserServiceImpl sınıfı, gRPC ile kullanıcı servislerinin sunulduğu bir sınıftır.
    public class UserServiceImpl : UserService.UserService.UserServiceBase
    {
        // Veritabanı bağlamını temsil eden özel bir alan.
        private readonly GrpcUserServiceContext _context;

        // Constructor metodu, veritabanı bağlamını alır ve sınıf içinde kullanmak üzere atar.
        public UserServiceImpl(GrpcUserServiceContext context)
        {
            _context = context;
        }

        // GetUser metodu, belirtilen ID'ye sahip bir kullanıcıyı veritabanından çeker.
        public override async Task<UserResponse> GetUser(UserRequest request, ServerCallContext context)
        {
            // Veritabanından kullanıcı arama işlemi yapılır.
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.Id);
            // Eğer kullanıcı bulunamazsa hata fırlatılır.
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            // Kullanıcı bulunduysa, gerekli bilgileri içeren bir UserResponse döndürülür.
            return new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                PasswordHash = user.PasswordHash
            };
        }

        // CreateUser metodu, yeni bir kullanıcı oluşturur.
        public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            // Yeni kullanıcı nesnesi oluşturulur.
            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = request.PasswordHash
            };

            // Yeni kullanıcı veritabanına eklenir.
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // İşlem başarılı olursa, oluşturulan kullanıcının ID'si ile birlikte başarı mesajı döndürülür.
            return new CreateUserResponse
            {
                Id = newUser.Id, // Yeni oluşturulan kullanıcının ID'si.
                Message = "User created successfully"
            };
        }

        // UpdateUser metodu, mevcut bir kullanıcının bilgilerini günceller.
        public override async Task<UpdateUserRequest> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            // Veritabanından ilgili kullanıcı aranır.
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.Id);
            // Kullanıcı bulunamazsa hata fırlatılır.
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            // Kullanıcı bulunduysa, bilgileri güncellenir.
            user.Username = request.Username;
            user.Email = request.Email;
            user.PasswordHash = request.PasswordHash;

            // Değişiklikler kaydedilir.
            await _context.SaveChangesAsync();

            // Güncellenen istek nesnesi geri döndürülür.
            return request; // Geri dönüş değeri olarak güncellenen istek nesnesi kullanılır.
        }

        // DeleteUser metodu, belirtilen ID'ye sahip kullanıcıyı veritabanından siler.
        public override async Task<DeleteUserRequest> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            // Veritabanından ilgili kullanıcı aranır.
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.Id);
            // Kullanıcı bulunamazsa hata fırlatılır.
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            // Kullanıcı bulunduysa, veritabanından silinir.
            _context.Users.Remove(user);
            // Değişiklikler kaydedilir.
            await _context.SaveChangesAsync();

            // Geri dönüş değeri olarak silinen istek nesnesi kullanılır.
            return request; // Silinen istek nesnesi geri döndürülür.
        }
    }
}
