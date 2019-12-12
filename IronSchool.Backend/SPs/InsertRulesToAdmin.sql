INSERT INTO RoleRules(RuleId, RoleId)
SELECT RuleId, 1
FROM [Rule]
WHERE not RuleId IN (select RuleId from RoleRules where RoleId = 1)