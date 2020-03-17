using System.Linq;
using System.Threading.Tasks;
using YukiDrive.Contexts;
using YukiDrive.Models;

namespace YukiDrive.Services
{
    public class SettingService
    {
        SettingContext context;
        public SettingService(SettingContext context){
            this.context = context;
        }

        /// <summary>
        /// 获取设置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key){
            var result = context.settings.SingleOrDefault(setting => setting.Key == key).Value;
            if(result == null){
                return null;
            }
            return result;
        }

        /// <summary>
        /// 创建或更新设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task Set(string key,string value){
            //已经存在
            if(context.settings.Any(setting => setting.Key == key)){
                Setting setting = context.settings.Single(setting => setting.Key == value);
                setting.Value = value;
                context.settings.Update(setting);
            } else {
                Setting setting = new Setting(){
                    Key = key,
                    Value = value
                };
                await context.settings.AddAsync(setting);
            }
            await context.SaveChangesAsync();
        }
    }
}