-- ============================================================
-- 种子数据：初始化系统角色 + 管理员账号
-- 密码: Admin@123  (SHA256 哈希)
-- 运行方式: 在 init.sql 执行完毕后运行此脚本
-- ============================================================

USE `letiao_ats`;

-- 角色字典
INSERT IGNORE INTO `sys_role` (`role_name`, `role_code`) VALUES
('系统管理员', 'ROLE_ADMIN'),
('招聘HR',     'ROLE_HR'),
('用人经理',   'ROLE_BIZ_MGR');

-- 初始管理员账号（密码 Admin@123 的 SHA256 小写哈希）
INSERT IGNORE INTO `sys_user` (`username`, `password_hash`, `real_name`, `department_name`, `status`) VALUES
('admin', 'e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7', '系统管理员', '信息技术部', 1);

-- 绑定管理员角色
INSERT IGNORE INTO `sys_user_role` (`user_id`, `role_id`)
SELECT u.id, r.id
FROM sys_user u, sys_role r
WHERE u.username = 'admin' AND r.role_code = 'ROLE_ADMIN';
