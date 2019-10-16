﻿

using Microsoft.Extensions.Logging;
using System;

namespace DIChainOfResponsibility
{
    /// <summary>
    /// 第三个任务
    /// </summary>
    public class ThirdTask : ParentTask
    {
        readonly ILogger<ThirdTask> _logger;
        public ThirdTask(ILogger<ThirdTask> logger, EndTask endTask)
        {
            this.Next(endTask);
            _logger = logger;
        }
        /// <summary>
        /// 职责链任务方法
        /// </summary>
        /// <param name="transferParmeter">任务内容</param>
        /// <returns></returns>
        public override bool ExecuteTask(TaskParmeter taskParmeter)
        {
            var result = SelfTask(taskParmeter);
            return _parentTask.ExecuteTask(taskParmeter) && result;
        }
        bool SelfTask(TaskParmeter  taskParmeter)
        {
            _logger.LogInformation("-------------------------------------------ThirdTask");
            return true;
        }
    }
}
