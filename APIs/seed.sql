-- Seed script: inserts 5 companies and 210 job listings for EXPLAIN ANALYZE testing.
-- Run in psql: \i seed.sql

-- Companies
INSERT INTO companies ("CompanyId", "CompanyName", "Industry") VALUES
  ('aaaaaaaa-0000-0000-0000-000000000001', 'Acme Corp',        'Technology'),
  ('aaaaaaaa-0000-0000-0000-000000000002', 'BuildRight Ltd',   'Construction'),
  ('aaaaaaaa-0000-0000-0000-000000000003', 'ClearData Inc',    'Finance'),
  ('aaaaaaaa-0000-0000-0000-000000000004', 'DevStream SA',     'Software'),
  ('aaaaaaaa-0000-0000-0000-000000000005', 'EduPath Group',    'Education')
ON CONFLICT DO NOTHING;

-- Job listings: 210 rows across 5 companies, mix of Active/Closed, varied dates.
-- Titles include deliberate keywords for full-text search proof:
--   "sprint" appears in exactly 3 listings (to demonstrate stemming: "sprinting" also matches)
INSERT INTO job_listings
  ("Id", "Title", "Description", "CompanyId", "Location", "Type", "ClosingDate", "SalaryMin", "SalaryMax", "PostedAt", "IsActive")
SELECT
  gen_random_uuid(),
  CASE (gs % 42)
    WHEN 0  THEN 'Senior Software Engineer'
    WHEN 1  THEN 'Junior Developer'
    WHEN 2  THEN 'Product Manager'
    WHEN 3  THEN 'Data Analyst'
    WHEN 4  THEN 'DevOps Engineer'
    WHEN 5  THEN 'UX Designer'
    WHEN 6  THEN 'QA Engineer'
    WHEN 7  THEN 'Backend Developer'
    WHEN 8  THEN 'Frontend Developer'
    WHEN 9  THEN 'Scrum Master – leads sprint planning'
    WHEN 10 THEN 'Agile Coach focused on sprinting methodology'
    WHEN 11 THEN 'Project Lead with sprinting experience required'
    WHEN 12 THEN 'Systems Architect'
    WHEN 13 THEN 'Cloud Engineer'
    WHEN 14 THEN 'Security Analyst'
    WHEN 15 THEN 'Technical Writer'
    WHEN 16 THEN 'Database Administrator'
    WHEN 17 THEN 'Machine Learning Engineer'
    WHEN 18 THEN 'Site Reliability Engineer'
    WHEN 19 THEN 'Mobile Developer'
    WHEN 20 THEN 'Full Stack Developer'
    WHEN 21 THEN 'Business Analyst'
    WHEN 22 THEN 'IT Support Specialist'
    WHEN 23 THEN 'Network Engineer'
    WHEN 24 THEN 'Compliance Officer'
    WHEN 25 THEN 'Financial Analyst'
    WHEN 26 THEN 'Recruitment Specialist'
    WHEN 27 THEN 'Marketing Manager'
    WHEN 28 THEN 'Content Strategist'
    WHEN 29 THEN 'Operations Manager'
    WHEN 30 THEN 'Customer Success Manager'
    WHEN 31 THEN 'Sales Engineer'
    WHEN 32 THEN 'Solution Architect'
    WHEN 33 THEN 'Platform Engineer'
    WHEN 34 THEN 'Infrastructure Lead'
    WHEN 35 THEN 'Embedded Systems Developer'
    WHEN 36 THEN 'Game Developer'
    WHEN 37 THEN 'Blockchain Developer'
    WHEN 38 THEN 'AI Research Scientist'
    WHEN 39 THEN 'Data Engineer'
    WHEN 40 THEN 'ETL Developer'
    ELSE         'Internship – Graduate Programme'
  END || ' #' || gs,
  'We are looking for a talented professional to join our growing team. ' ||
  'You will work closely with cross-functional teams to deliver high-quality solutions. ' ||
  'Experience with modern tooling and agile practices is essential.',
  -- Distribute evenly across 5 companies
  CASE (gs % 5)
    WHEN 0 THEN 'aaaaaaaa-0000-0000-0000-000000000001'::uuid
    WHEN 1 THEN 'aaaaaaaa-0000-0000-0000-000000000002'::uuid
    WHEN 2 THEN 'aaaaaaaa-0000-0000-0000-000000000003'::uuid
    WHEN 3 THEN 'aaaaaaaa-0000-0000-0000-000000000004'::uuid
    ELSE        'aaaaaaaa-0000-0000-0000-000000000005'::uuid
  END,
  CASE (gs % 4)
    WHEN 0 THEN 'Cape Town'
    WHEN 1 THEN 'Johannesburg'
    WHEN 2 THEN 'Durban'
    ELSE        'Remote'
  END,
  (gs % 4),  -- JobType enum: 0=FullTime,1=PartTime,2=Contract,3=Internship
  -- Mix: 70% future closing dates (active), 30% past (closed)
  CASE WHEN gs % 10 < 7
    THEN NOW() + (((gs % 90) + 5) || ' days')::interval
    ELSE NOW() - (((gs % 30) + 1) || ' days')::interval
  END,
  CASE WHEN gs % 3 = 0 THEN NULL ELSE (15000 + (gs % 20) * 2500)::numeric END,
  CASE WHEN gs % 3 = 0 THEN NULL ELSE (35000 + (gs % 20) * 3000)::numeric END,
  NOW() - (((gs % 60)) || ' days')::interval,
  -- IsActive aligns with ClosingDate: future = active, past = inactive
  CASE WHEN gs % 10 < 7 THEN true ELSE false END
FROM generate_series(1, 210) gs;
