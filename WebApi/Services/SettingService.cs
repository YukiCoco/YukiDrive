using System;
using System.Linq;
using System.Threading.Tasks;
using YukiDrive.Contexts;
using YukiDrive.Models;

namespace YukiDrive.Services
{
    public class SettingService: IDisposable
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
            //判断是否存在
            if(!context.Settings.Any(setting => setting.Key == key)){
                return null;
            }
            var result = context.Settings.SingleOrDefault(setting => setting.Key == key).Value;
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
            if(context.Settings.Any(setting => setting.Key == key)){
                Setting setting = context.Settings.Single(setting => setting.Key == key);
                setting.Value = value;
                context.Settings.Update(setting);
            } else {
                Setting setting = new Setting(){
                    Key = key,
                    Value = value
                };
                await context.Settings.AddAsync(setting);
            }
            await context.SaveChangesAsync();
        }

        public void Dispose(){
            context.Dispose();
        }
    }
}